namespace Voycar.Api.Web.Generic.Repository;

public interface IRepository<T> where T : Entity
{
    // CRUD operations
    Guid? Create(T entity);
    T? Retrieve(Guid id);
    bool Update(T entity);
    bool Delete(Guid id);

    IEnumerable<T> RetrieveAll();
    // Alternative to Create() which only allows creation of entirely unique entities
    Guid? CreateUnique(T entity);
}
