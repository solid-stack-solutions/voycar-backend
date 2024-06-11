namespace Voycar.Api.Web.Features.Roles.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Role>
{
    // ToDo roles
    public Single(IRoles roles) : base(roles, ["admin"]) {}
}
