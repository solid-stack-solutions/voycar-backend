namespace Voycar.Api.Web.Features.Permissions.Repository;

using Context;
using Entities;
using Generic.Repository;

public class Roles : Repository<Role>, IRoles
{
    public Roles(VoycarDbContext context) : base(context) {}
}
