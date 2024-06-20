namespace Voycar.Api.Web.Features.Members.Endpoints.Post;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Post.Single<Member>
{
    // ToDo roles
    public Single(IMembers repository) : base(repository, ["admin"]) {}
}
