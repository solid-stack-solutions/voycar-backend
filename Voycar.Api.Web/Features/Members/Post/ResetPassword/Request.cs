namespace Voycar.Api.Web.Features.Members.Post.ResetPassword;

public class Request
{
    public string PasswordResetToken { get; set; }
    public string Password { get; set; }
}
