namespace Voycar.Api.Web.Features.Stations.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Station>
{
    // ToDo roles
    public Single(IStations repository) : base(repository, ["admin"]) {}
}
