namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public Guid Id { get; set; }
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
    public required string City { get; set; }

    // Foreign key to Role
    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public Role Role { get; set; } // Navigation property, useful for Repository

    // Reset
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
}
