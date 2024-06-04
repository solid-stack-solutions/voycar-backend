namespace Voycar.Api.Web.Features.Members.Post.Login;

using System.ComponentModel.DataAnnotations;

public class Request
{
    // Login
    [EmailAddress]
    public string? Email { get; set; }
    public string? Password { get; set; }
}
