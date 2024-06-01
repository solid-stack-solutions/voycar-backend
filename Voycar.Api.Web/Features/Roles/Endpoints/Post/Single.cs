namespace Voycar.Api.Web.Features.Roles.Endpoints.Post;

using Entities;
using Repository;

public class Endpoint : Generic.Endpoint.Post.Single<Role>
{
    //TODO roles
    public Endpoint(IRoles roles) : base(roles, ["admin"] ) {}
}
