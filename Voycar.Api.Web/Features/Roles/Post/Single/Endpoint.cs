namespace Voycar.Api.Web.Features.Roles.Post.Single;

using Entities;
using Repository;

public class Endpoint : Generic.Endpoint.Post.Single<Role, Request, Mapper>
{
    public Endpoint(IRoles roles) : base(roles, "/role/single") {}
}
