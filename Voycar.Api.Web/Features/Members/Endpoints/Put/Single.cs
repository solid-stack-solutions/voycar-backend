namespace Voycar.Api.Web.Features.Members.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Member>
{
    // ToDo roles
    public Single(IMembers repository) : base(repository, ["admin"]) {}
}
