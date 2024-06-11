namespace Voycar.Api.Web.Features.Plans.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Plan>
{
    // ToDo roles
    public Single(IPlans repository) : base(repository, ["admin"]) {}
}
