namespace Voycar.Api.Web.Features.Permissions.Repository;

using Context;
using Entities;

public class Permissions : IPermissions
{
    private readonly VoycarDbContext _context;

    public Permissions(VoycarDbContext context)
    {
        this._context = context;
    }

    /// <returns>
    ///     <c>true</c> if permission was created
    /// </returns>
    public bool CreatePermission(Permission permission)
    {
        this._context.Permissions.Add(permission);
        var numChanges = this._context.SaveChanges();
        // true when something changed
        return numChanges > 0;
    }
}
