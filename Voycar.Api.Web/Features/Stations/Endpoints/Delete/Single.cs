namespace Voycar.Api.Web.Features.Stations.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Station>
{
    // ToDo roles
    public Single(IStations repository) : base(repository, ["admin"]) {}
}
