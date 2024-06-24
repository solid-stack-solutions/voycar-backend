namespace Voycar.Api.Web.Features.Cities.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<City>
{
    public SingleUnique(ICities repository) : base(repository, ["admin"]) {}
}
