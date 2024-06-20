namespace Voycar.Api.Web.Features.Users.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<User>
{
    // ToDo roles
    public All(IUsers repository) : base(repository, ["admin"]) {}
}
