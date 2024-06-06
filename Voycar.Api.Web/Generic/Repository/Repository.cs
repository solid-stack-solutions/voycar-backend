namespace Voycar.Api.Web.Generic.Repository;

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
    ///     exceptions from <c>SaveChanges()</c> are ignored.
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
        // ignore some possible exception,
        // e.g. when trying to create an entity with a duplicate key
        finally {}
        return numChanges > 0;
    }

    /// <returns>
    ///     <c>true</c> if entity was created
    /// </returns>
    public bool Create(TEntity entity)
    {
        this.dbSet.Add(entity);
        return this.SaveChanges();
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
    ///     Only create entity if it's not already in the database.
    ///     More specifically: If the database does not contain
    ///     an entity with the exact same attribute values.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if entity was created
    /// </returns>
    public bool CreateUnique(TEntity entity)
    {
        // check if entity already exists
        if (this.dbSet.Any(e => e.Equals(entity)))
        {
            return false;
        }

        return this.Create(entity);
    }
}
