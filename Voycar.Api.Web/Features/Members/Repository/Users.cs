namespace Voycar.Api.Web.Features.Members.Repository;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;


public class Users : Generic.Repository.Repository<User>, IUsers
{
    public Users(VoycarDbContext context) : base(context) {}


    public Task<User?> Retrieve(string attribute, string? value)
    {
        return attribute switch
        {
            "email" => this.dbSet.FirstOrDefaultAsync(
                user => user.Email == value),

            "passwordResetToken" => this.dbSet.FirstOrDefaultAsync(
                user => user.PasswordResetToken == value),

            _ => throw new ArgumentException("Invalid property name", nameof(attribute))
        };
    }
}
