namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generic;


public class User : Entity
{
    // Login information
    [EmailAddress]
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    // Verify user
    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }

    // Password reset
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }

    // User role foreign key
    [ForeignKey("Role")]
    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }

    // Member information foreign key
    [ForeignKey("Member")]
    public Guid? MemberId { get; set; }
    public Member? Member { get; set; }
}
