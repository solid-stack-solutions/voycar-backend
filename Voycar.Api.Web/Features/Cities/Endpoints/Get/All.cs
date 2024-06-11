namespace Voycar.Api.Web.Features.Cities.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<City>
{
    // ToDo roles
    public All(ICities repository) : base(repository, ["admin"]) {}
}
