namespace Voycar.Api.Web.Features.Users.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<User>
{
    public Single(IUsers repository) : base(repository, ["admin", "employee"]) {}
}
