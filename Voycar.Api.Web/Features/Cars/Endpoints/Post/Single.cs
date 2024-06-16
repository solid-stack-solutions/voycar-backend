namespace Voycar.Api.Web.Features.Cars.Endpoints.Post;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Post.Single<Car>
{
    // ToDo roles
    public Single(ICars repository) : base(repository, ["admin"]) {}
}
