namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.ComponentModel.DataAnnotations;

public class Request
{
    // Login
    [EmailAddress]
    public required string Email{ get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }

    // Personal Details
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Street { get; set; }
    public required string HouseNumber { get; set; }
    public required string PostalCode { get; set; }
    public required string Place { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required string BirthPlace { get; set; }
    public required string PhoneNumber { get; set; }
    public required string? DriversLicenseNumber { get; set; }
    public required string IdCardNumber { get; set; }
}
