namespace Voycar.Api.Web.Features.Members.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Member>
{
    // ToDo roles
    public Single(IMembers repository) : base(repository, ["admin"]) {}
}
