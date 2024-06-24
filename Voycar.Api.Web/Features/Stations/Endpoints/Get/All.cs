namespace Voycar.Api.Web.Features.Stations.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<Station>
{
    public All(IStations repository) : base(repository, ["admin", "employee", "member"]) {}
}
