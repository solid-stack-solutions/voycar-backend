namespace Voycar.Api.Web.Features.Cars.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Car>
{
    // ToDo roles
    public Single(ICars repository) : base(repository, ["admin"]) {}
}
