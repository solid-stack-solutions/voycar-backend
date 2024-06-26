namespace Voycar.Api.Web.Features.Plans.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Plan>
{
    public Single(IPlans repository) : base(repository, ["admin"]) {}
}
