namespace Voycar.Api.Web.Features.Stations.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Station>
{
    public SingleUnique(IStations repository) : base(repository, ["admin"]) {}
}
