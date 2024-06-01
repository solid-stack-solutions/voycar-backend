namespace Voycar.Api.Web.Features.Members.Post.ForgotPassword;

using System.ComponentModel.DataAnnotations;

public class Request
{
    [EmailAddress]
    public required string Email{ get; set; }
}
