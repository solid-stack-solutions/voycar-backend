namespace Voycar.Api.Web.Features.Plans.Endpoints.Put.Personal;

public class Request
{
    [FromClaim]
    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
}
