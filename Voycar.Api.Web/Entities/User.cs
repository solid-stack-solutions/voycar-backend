namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generic;

public class User : Entity
{
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
    public Guid RoleId { get; set; }
    public Role Role { get; set; } // Navigation property, useful for Repository
}
