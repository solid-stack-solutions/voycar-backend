namespace Voycar.Api.Web.Features.Plans.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Plan>
{
    public Single(IPlans repository) : base(repository, []) {}
}
