namespace Voycar.Api.Web.Features.Reservations.Endpoints.Get.Personal;

public class Request
{
    [FromClaim]
    public Guid UserId { get; set; }
}
