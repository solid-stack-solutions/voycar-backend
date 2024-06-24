namespace Voycar.Api.Web.Features.Cars.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Car>
{
    public SingleUnique(ICars repository) : base(repository, ["admin"]) {}
}
