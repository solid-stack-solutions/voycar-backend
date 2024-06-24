namespace Voycar.Api.Web.Features.Roles.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<Role>
{
    public All(IRoles repository) : base(repository, ["admin"]) {}
}
