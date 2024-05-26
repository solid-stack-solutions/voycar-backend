namespace Voycar.Api.Web.Features.Permissions.Repository;

using Entities;

public interface IPermissions
{
    bool CreatePermission(Permission permission);
}
