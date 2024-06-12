namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetToken;

public class Request
{
    [EmailAddress]
    public string Email { get; set; }
}
