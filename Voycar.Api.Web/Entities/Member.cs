namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Member
{
    [Key, ForeignKey("User")]
    public Guid UserId { get; set; }
    public required DateOnly BirthDate { get; set; }
    public required string BirthPlace { get; set; }
    public required string PhoneNumber { get; set; }

    public int TierId { get; set; }
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

    public User User { get; set; } // Set the navigation property, useful for Repository
}
