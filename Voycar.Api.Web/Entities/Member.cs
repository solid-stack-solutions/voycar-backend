namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generic;

public class Member : Entity
{


    public DateOnly BirthDate { get; set; }
    public string BirthPlace { get; set; }
    public string PhoneNumber { get; set; }

    public int TierId { get; set; }
    public string? DriversLicenseNumber { get; set; }
    public bool ValidDriversLicense { get; set; }
    public string IdCardNumber { get; set; }
    public bool ValidPostIdent { get; set; }

    // Verification
    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }

    public User User { get; set; } // Set the navigation property, useful for Repository
}
