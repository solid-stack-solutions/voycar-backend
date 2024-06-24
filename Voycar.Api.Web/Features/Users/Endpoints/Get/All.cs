namespace Voycar.Api.Web.Features.Users.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<User>
{
    public All(IUsers repository) : base(repository, ["admin", "employee"]) {}
}
