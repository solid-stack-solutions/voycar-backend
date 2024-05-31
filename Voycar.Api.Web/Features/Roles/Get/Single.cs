namespace Voycar.Api.Web.Features.Roles.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Role>
{
    public Single(IRoles repository) : base(repository, ["admin", "member", "guest"] ) {}
}
