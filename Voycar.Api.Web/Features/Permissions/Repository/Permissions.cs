namespace Voycar.Api.Web.Features.Permissions.Repository;

using Context;
using Entities;
using Generic.Repository;

public class Permissions : Repository<Permission>, IPermissions
{
    public Permissions(VoycarDbContext context) : base(context) {}
}
