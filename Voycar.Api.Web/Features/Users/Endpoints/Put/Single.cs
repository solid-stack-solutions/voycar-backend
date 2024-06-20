namespace Voycar.Api.Web.Features.Users.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<User>
{
    // ToDo roles
    public Single(IUsers repository) : base(repository, ["admin"]) {}
}
