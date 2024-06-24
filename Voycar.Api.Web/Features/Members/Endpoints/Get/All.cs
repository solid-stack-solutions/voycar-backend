namespace Voycar.Api.Web.Features.Members.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<Member>
{
    public All(IMembers repository) : base(repository, ["admin", "employee"]) {}
}
