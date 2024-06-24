namespace Voycar.Api.Web.Features.Cars.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<Car>
{
    public All(ICars repository) : base(repository, ["admin", "employee", "member"]) {}
}
