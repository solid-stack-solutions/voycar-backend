namespace Voycar.Api.Web.Features.Reservations.Endpoints.Delete.Personal;

public class Request
{
    [FromClaim]
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
}
