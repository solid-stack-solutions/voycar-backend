namespace Voycar.Api.Web.Features.Roles.Delete;

using Entities;
using Repository;

public class Endpoint : Generic.Endpoint.Delete.Single<Role>
{
    //TODO roles
    public Endpoint(IRoles repository) : base(repository, [ "admin" ] ) {}
}
