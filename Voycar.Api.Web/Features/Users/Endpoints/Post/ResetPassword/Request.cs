namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetPassword;

public class Request
{
    public string PasswordResetToken { get; set; }
    public string Password { get; set; }
}
