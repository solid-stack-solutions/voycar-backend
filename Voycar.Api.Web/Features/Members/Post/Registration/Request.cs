namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.ComponentModel.DataAnnotations;


public class Request
{
    // Login information
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }
}
