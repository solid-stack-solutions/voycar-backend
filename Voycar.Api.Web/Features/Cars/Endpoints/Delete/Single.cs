namespace Voycar.Api.Web.Features.Cars.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Car>
{
    // ToDo roles
    public Single(ICars repository) : base(repository, ["admin"]) {}
}
