namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;

public class Member
{
    public int Id { get; set; }

    // Login
    [EmailAddress]
    public required string Email{ get; set; }
    public string PasswordHash { get; set; }

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
    public int RoleId { get; set; }
    public int TariffId { get; set; }
    public required string? DriversLicenseNumber { get; set; }
    public bool ValidDriversLicense { get; set; }
    public required string IdCardNumber { get; set; }
    public bool ValidPostIdent { get; set; }


    // Verification
    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }

    // Reset
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
}
