namespace Voycar.Api.Web.Features.Roles.Repository;

using Context;
using Entities;
using Microsoft.EntityFrameworkCore;

public class Roles : Generic.Repository.Repository<Role>, IRoles
{
    public Roles(VoycarDbContext context) : base(context) {}

    public Task<Role?> Retrieve(string name)
    {
        return this.DbSet.FirstOrDefaultAsync(
            role => role.Name == name);
    }
}
