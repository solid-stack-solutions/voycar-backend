namespace Voycar.Api.Web.Features.Roles.Post;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Post.Single<Role>
{
    //TODO roles
    public Single(IRoles roles) : base(roles, ["admin"] ) {}
}
