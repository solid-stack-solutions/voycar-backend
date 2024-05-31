namespace Voycar.Api.Web.Features.Roles.Get;

using Entities;
using Repository;

public class Endpoint : Generic.Endpoint.Get.Single<Role>
{
    // TODO roles
    public Endpoint(IRoles repository) : base(repository, ["admin", "member", "guest"] ) {}
}
