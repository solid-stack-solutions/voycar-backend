namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.Security.Cryptography;
using Entities;

public class Mapper : Mapper<Request, Response, Member>
{

    public override Member ToEntity(Request r)
    {
        // Create the User entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = r.Email,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(r.Password),
            FirstName = r.FirstName,
            LastName = r.LastName,
            Street = r.Street,
            HouseNumber = r.HouseNumber,
            PostalCode = r.PostalCode,
            City = r.City,
            RoleId = 1 // Sets RoleId to member as default
        };

        // Create the Member entity and link it to the User entity
        var member = new Member
        {
            UserId = user.Id, // Set the foreign key to the User's Id
            User = user,      // Set the navigation property, useful for Repository
            BirthDate = r.BirthDate,
            BirthPlace = r.BirthPlace,
            PhoneNumber = r.PhoneNumber,
            DriversLicenseNumber = r.DriversLicenseNumber,
            IdCardNumber = r.IdCardNumber,
            VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(256))
        };

        return member;
    }
}
