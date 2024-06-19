namespace Voycar.Api.Web.Features.Users.Endpoints.Get.WhoAmI;

public class Request
{
    [FromClaim]
    public Guid UserId { get; set; }
}
