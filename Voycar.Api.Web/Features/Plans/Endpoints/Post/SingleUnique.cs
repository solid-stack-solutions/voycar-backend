namespace Voycar.Api.Web.Features.Plans.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Plan>
{
    public SingleUnique(IPlans repository) : base(repository, ["admin"]) {}
}
