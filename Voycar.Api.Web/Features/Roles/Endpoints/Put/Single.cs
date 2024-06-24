namespace Voycar.Api.Web.Features.Roles.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Role>
{
    public Single(IRoles repository) : base(repository, ["admin"]) {}
}
