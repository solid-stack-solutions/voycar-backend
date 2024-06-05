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

    public Task<User?> Retrieve(string email)
    {
        return this._context.Users.FirstOrDefaultAsync(
            user => user.Email == email.ToLowerInvariant());
    }
}
