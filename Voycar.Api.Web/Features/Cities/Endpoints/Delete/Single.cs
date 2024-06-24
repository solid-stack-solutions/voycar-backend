namespace Voycar.Api.Web.Features.Cities.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<City>
{
    public Single(ICities repository) : base(repository, ["admin"]) {}
}
