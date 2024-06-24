namespace Voycar.Api.Web.Features.Stations.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Station>
{
    public Single(IStations repository) : base(repository, ["admin", "employee", "member"]) {}
}
