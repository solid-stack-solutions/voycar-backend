namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.ComponentModel.DataAnnotations;


public class Request
{
    // User information
    [EmailAddress]
    public string Email { get; set; }
    public string Password { get; set; }

    // Member information
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateOnly BirthDate { get; set; }
    public string BirthPlace { get; set; }
    public string PhoneNumber { get; set; }
}
