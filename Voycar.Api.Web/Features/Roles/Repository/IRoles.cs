namespace Voycar.Api.Web.Features.Roles.Repository;

using Entities;

public interface IRoles : Generic.Repository.IRepository<Role>
{
    Task<Role?> Retrieve(string name);
}
