namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Login;

public class Request
{
    // Login
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}
