namespace Voycar.Api.Web.Features.Members.Post.ResetPassword;

using System.ComponentModel.DataAnnotations;

public class Request
{
    [Required]
    public required string PasswordResetToken { get; set; }
    [Required]
    public required string Password { get; set; }
}
