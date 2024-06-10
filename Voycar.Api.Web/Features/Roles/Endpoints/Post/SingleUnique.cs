namespace Voycar.Api.Web.Features.Roles.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Role>
{
    // ToDo roles
    public SingleUnique(IRoles roles) : base(roles, ["admin"]) {}
}
