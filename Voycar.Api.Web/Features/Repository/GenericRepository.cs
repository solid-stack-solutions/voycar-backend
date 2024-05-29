namespace Voycar.Api.Web.Features.Repository;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> : IGenericRepository<T> where T : GenericEntity
{
    private readonly VoycarDbContext _context;
    private readonly DbSet<T> dbSet;

    public GenericRepository(VoycarDbContext context)
    {
        this._context = context;
        this.dbSet = this._context.Set<T>();
    }

    /// <returns>
    ///     <c>true</c> if database changed
    /// </returns>
    private bool SaveChanges()
    {
        var numChanges = 0;
        numChanges = this._context.SaveChanges();
        return numChanges > 0;
    }

    /// <returns>
    ///     <c>true</c> if entity was created
    /// </returns>
    public bool Create(T entity)
    {
        this.dbSet.Add(entity);
        var returnValue = false;
        try
        {
            returnValue = this.SaveChanges();
        }
        // ignore possible duplicate key exception
        finally {}
        return returnValue;
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
