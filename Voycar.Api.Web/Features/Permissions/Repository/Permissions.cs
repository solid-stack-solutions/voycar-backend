namespace Voycar.Api.Web.Features.Permissions.Repository;

using Context;
using Entities;
using Features.Repository;

public class Permissions : GenericRepository<Permission>, IPermissions
{
    public Permissions(VoycarDbContext context) : base(context) {}
}
