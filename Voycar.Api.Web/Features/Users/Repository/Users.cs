namespace Voycar.Api.Web.Features.Users.Repository;

using Context;
using Entities;

public class Users : Generic.Repository.Repository<User>, IUsers
{
    public Users(VoycarDbContext context) : base(context) {}


    public Task<User?> RetrieveByVerificationToken(string verificationToken)
    {
        return this._context.Users.FirstOrDefaultAsync(member => member.VerificationToken == verificationToken);
    }

    public Task<User?> RetrieveByEmail(string email)
    {
        return this.dbSet.FirstOrDefaultAsync(user => user.Email == email);
    }

    public Task<User?> RetrieveByPasswordResetToken(string resetToken)
    {
        return this.dbSet.FirstOrDefaultAsync(user => user.PasswordResetToken == resetToken);
    }
}
