namespace Voycar.Api.Web.Features.Users.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<User>
{
    // ToDo roles
    public SingleUnique(IUsers repository) : base(repository, ["admin"]) {}
}
