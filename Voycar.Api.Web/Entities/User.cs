namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generic;

public class User : Entity
{
    // Login
    [EmailAddress]
    public string Email{ get; set; }
    public string PasswordHash { get; set; }

    // Personal Details
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }

    // Foreign key to Role
    [ForeignKey("Role")]
    public Guid RoleId { get; set; }
    public Role Role { get; set; } // Navigation property, useful for Repository and necessary for relationship

    // Reset
    public string? PasswordResetToken { get; set; }
    public DateTime ResetTokenExpires { get; set; }
}
