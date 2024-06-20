namespace Voycar.Api.Web.Features.Members.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Member>
{
    // ToDo roles
    public Single(IMembers repository) : base(repository, ["admin"]) {}
}
