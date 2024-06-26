namespace Voycar.Api.Web.Features.Stations.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Station>
{
    public Single(IStations repository) : base(repository, ["admin"]) {}
}
