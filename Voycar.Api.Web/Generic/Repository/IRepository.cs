namespace Voycar.Api.Web.Generic.Repository;

using Entities;

public interface IRepository<T> where T : Entity
{
    // CRUD operations
    bool Create(T entity);
    T? Retrieve(Guid id);
    bool Update(T entity);
    bool Delete(Guid id);

    IEnumerable<T> RetrieveAll();
}
