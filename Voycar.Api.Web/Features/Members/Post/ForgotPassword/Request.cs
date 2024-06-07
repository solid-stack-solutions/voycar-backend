namespace Voycar.Api.Web.Features.Members.Post.ForgotPassword;

using System.ComponentModel.DataAnnotations;


public class Request
{
    [EmailAddress] public string? Email { get; set; }
}
