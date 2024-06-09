namespace Voycar.Api.Web.Features.Members.Post.Registration;

using System.Security.Cryptography;
using Entities;


public class Mapper : RequestMapper<Request, User>
{

    public override User ToEntity(Request r)
    {
        // Create the User entity
        var user = new User
        {
            Email = r.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(r.Password),

            VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(256))
        };

        return user;
    }
}
