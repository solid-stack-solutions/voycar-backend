namespace Voycar.Api.Web.Features.Reservations.Endpoints.Delete.Personal;

public class Request
{
    [FromClaim]
    public Guid UserId { get; set; }
    // ID of the reservation to be deleted
    public Guid Id { get; set; }
}
