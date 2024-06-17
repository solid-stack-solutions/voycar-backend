namespace Voycar.Api.Web.Features.Users.Repository;

using Entities;

public interface IUsers : Generic.Repository.IRepository<User>
{
    Task<User?> RetrieveByVerificationToken(string verificationToken);
    Task<User?> RetrieveByEmail(string email);
    Task<User?> RetrieveByPasswordResetToken(string resetToken);
}
