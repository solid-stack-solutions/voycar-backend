namespace Voycar.Api.Web.Features.Cities.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<City>
{
    public All(ICities repository) : base(repository, ["admin", "employee", "member"]) {}

    public override void Configure()
    {
        base.Configure();
        // Manually correct grammar
        this.Summary(s => s.Summary = "Retrieve all Cities");
    }
}
