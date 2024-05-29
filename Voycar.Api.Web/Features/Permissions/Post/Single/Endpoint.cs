namespace Voycar.Api.Web.Features.Permissions.Post.Single;

using Entities;
using Repository;

public class Endpoint : Generic.Endpoint.Post.Single<Permission, Request, Mapper>
{
    public Endpoint(IPermissions permissions) : base(permissions, "/permission/single") {}
}
