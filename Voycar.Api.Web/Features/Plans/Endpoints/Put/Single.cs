namespace Voycar.Api.Web.Features.Plans.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Plan>
{
    // ToDo roles
    public Single(IPlans repository) : base(repository, ["admin"]) {}
}
