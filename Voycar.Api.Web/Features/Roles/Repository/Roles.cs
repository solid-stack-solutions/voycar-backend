namespace Voycar.Api.Web.Features.Roles.Repository;

using Context;
using Entities;
using Voycar.Api.Web.Generic.Repository;

public class Roles : Repository<Role>, IRoles
{
    public Roles(VoycarDbContext context) : base(context) {}
}
