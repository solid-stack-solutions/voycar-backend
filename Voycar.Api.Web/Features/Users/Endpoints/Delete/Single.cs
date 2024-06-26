namespace Voycar.Api.Web.Features.Users.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<User>
{
    public Single(IUsers repository) : base(repository, ["admin"]) {}
}
