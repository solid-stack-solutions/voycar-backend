namespace Voycar.Api.Web.Features.Members.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Member>
{
    public Single(IMembers repository) : base(repository, ["admin", "employee"]) {}
}
