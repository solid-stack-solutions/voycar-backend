namespace Voycar.Api.Web.Generic.Repository;

using Microsoft.EntityFrameworkCore;
using Context;
using Entities;

public class Repository<T> : IRepository<T> where T : Entity
{
    private readonly VoycarDbContext _context;
    private readonly DbSet<T> dbSet;

    protected Repository(VoycarDbContext context)
    {
        this._context = context;
        this.dbSet = this._context.Set<T>();
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
    public bool Create(T entity)
    {
        this.dbSet.Add(entity);
        return this.SaveChanges();
    }

    public T? Retrieve(Guid id)
    {
        return this.dbSet.Find(id);
    }

    /// <returns>
    ///     <c>true</c> if entity was updated
    /// </returns>
    public bool Update(T entity)
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

    public IEnumerable<T> RetrieveAll()
    {
        return this.dbSet.ToList();
    }
}
