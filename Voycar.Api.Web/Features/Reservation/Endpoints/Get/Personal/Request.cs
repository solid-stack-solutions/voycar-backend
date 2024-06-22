namespace Voycar.Api.Web.Features.Reservation.Endpoints.Get.Personal;

public class Request
{
    [FromClaim]
    public Guid UserId { get; set; }
}
