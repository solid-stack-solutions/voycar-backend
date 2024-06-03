namespace Voycar.Api.Web.Features.Roles.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Role>
{
    // TODO roles
    public Single(IRoles repository) : base(repository, ["admin"]) {}
}
