namespace Voycar.Api.Web.Generic.Repository;

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Context;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly VoycarDbContext _context;
    private readonly DbSet<TEntity> dbSet;

    protected Repository(VoycarDbContext context)
    {
        this._context = context;
        this.dbSet = this._context.Set<TEntity>();
    }

    /// <summary>
    ///     Exceptions from <c>SaveChanges()</c> are ignored.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if database changed
    /// </returns>
    private bool SaveChanges()
    {
        var numChanges = 0;
        try
        {
            numChanges = this._context.SaveChanges();
        }
        // Ignore some possible exception,
        // e.g. when trying to create an entity with a duplicate key
        catch {}
        return numChanges > 0;
    }

    /// <returns>
    ///     <see cref="Guid"/> of entity if created, otherwise <c>null</c>
    /// </returns>
    public Guid? Create(TEntity entity)
    {
        this.dbSet.Add(entity);
        return this.SaveChanges() ? entity.Id : null;
    }

    public TEntity? Retrieve(Guid id)
    {
        return this.dbSet.Find(id);
    }

    /// <returns>
    ///     <c>true</c> if entity was updated
    /// </returns>
    public bool Update(TEntity entity)
    {
        this.dbSet.Update(entity);
        return this.SaveChanges();
    }

    /// <returns>
    ///     <c>true</c> if entity was deleted
    /// </returns>
    public bool Delete(Guid id)
    {
        var entity = this.Retrieve(id);
        if (entity is null)
        {
            return false;
        }

        this.dbSet.Remove(entity);
        return this.SaveChanges();
    }

    public IEnumerable<TEntity> RetrieveAll()
    {
        return this.dbSet.ToList();
    }

    /// <summary>
    ///     Helper method for <see cref="CreateUnique"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if attribute values of entities are equal, but ignore the <see cref="Entity.Id"/>s.
    /// </returns>
    private static bool EntitiesAreEqual(TEntity e1, TEntity e2)
    {
        return typeof(TEntity)
            // Select public instance (not static) attributes of entity
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            // Ignore Id for comparison
            .Where(p => p.Name != "Id")
            // Return if all selected attributes are equal
            .All(p => Equals(p.GetValue(e1), p.GetValue(e2)));
    }

    /// <summary>
    ///     Only create entity if it's not already in the database.
    ///     More specifically: If the database does not contain
    ///     an entity with the exact same attribute values.
    /// </summary>
    /// <returns>
    ///     <see cref="Guid"/> of entity if created, otherwise <c>null</c>
    /// </returns>
    public Guid? CreateUnique(TEntity entity)
    {
        // Check if entity already exists.
        // AsEnumerable() is necessary for explicit client evaluation,
        // as Any() has no server-side translation.
        // https://learn.microsoft.com/en-us/ef/core/querying/client-eval
        if (this.dbSet.AsEnumerable().Any(e => EntitiesAreEqual(e, entity)))
        {
            return null;
        }

        return this.Create(entity);
    }
}
