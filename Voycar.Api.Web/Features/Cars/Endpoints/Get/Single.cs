namespace Voycar.Api.Web.Features.Cars.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Car>
{
    // ToDo roles
    public Single(ICars repository) : base(repository, ["admin"]) {}
}
