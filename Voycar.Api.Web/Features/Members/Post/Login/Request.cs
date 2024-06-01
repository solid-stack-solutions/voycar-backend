namespace Voycar.Api.Web.Features.Members.Post.Login;

using System.ComponentModel.DataAnnotations;

public class Request
{
    // Login
    [EmailAddress]
    public required string Email{ get; set; }
    public required string Password { get; set; }
}
