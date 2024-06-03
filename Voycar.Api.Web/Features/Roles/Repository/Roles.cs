namespace Voycar.Api.Web.Features.Roles.Repository;

using Context;
using Entities;

public class Roles : Generic.Repository.Repository<Role>, IRoles
{
    public Roles(VoycarDbContext context) : base(context) {}
}
