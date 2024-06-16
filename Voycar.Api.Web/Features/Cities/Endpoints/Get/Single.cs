namespace Voycar.Api.Web.Features.Cities.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<City>
{
    // ToDo roles
    public Single(ICities repository) : base(repository, ["admin"]) {}
}
