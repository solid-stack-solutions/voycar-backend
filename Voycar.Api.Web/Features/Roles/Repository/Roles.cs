namespace Voycar.Api.Web.Features.Roles.Repository;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;

public class Roles : Generic.Repository.Repository<Role>, IRoles
{
    private readonly VoycarDbContext _context;

    public Roles(VoycarDbContext context) : base(context)
    {
        this._context = context;
    }

    public Task<Role?> Retrieve(string name)
    {
        return this._context.Roles.FirstOrDefaultAsync(
            role => role.Name == name);
    }
}
