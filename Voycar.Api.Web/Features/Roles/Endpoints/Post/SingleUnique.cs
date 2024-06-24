namespace Voycar.Api.Web.Features.Roles.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Role>
{
    public SingleUnique(IRoles repository) : base(repository, ["admin"]) {}
}
