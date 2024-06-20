namespace Voycar.Api.Web.Features.Cars.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Car>
{
    // ToDo roles
    public SingleUnique(ICars repository) : base(repository, ["admin"]) {}
}
