namespace Voycar.Api.Web.Features.Plans.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<Plan>
{
    public All(IPlans repository) : base(repository, ["admin", "employee", "member"]) {}
}
