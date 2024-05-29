namespace Voycar.Api.Web.Features.Repository;

using Entities;

public interface IGenericRepository<T> where T : GenericEntity
{
    // CRUD operations
    bool Create(T entity);
    T? Retrieve(Guid id);
    bool Update(T entity);
    bool Delete(Guid id);

    IEnumerable<T> RetrieveAll();
}
