namespace Voycar.Api.Web.Features.Members.Repository;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;


public class Users : Generic.Repository.Repository<User>, IUsers
{
    private readonly VoycarDbContext _context;


    public Users(VoycarDbContext context) : base(context)
    {
        _context = context;
    }


    public Task<User?> Retrieve(string attribute, string? value)
    {
        return attribute switch
        {
            "email" => this._context.Users.FirstOrDefaultAsync(
                user => user.Email == value),

            "passwordResetToken" => this._context.Users.FirstOrDefaultAsync(
                user => user.PasswordResetToken == value),

            _ => throw new ArgumentException("Invalid property name", nameof(attribute))
        };
    }
}
