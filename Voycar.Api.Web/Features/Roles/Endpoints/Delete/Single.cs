namespace Voycar.Api.Web.Features.Roles.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Role>
{
    public Single(IRoles repository) : base(repository, ["admin"]) {}
}
