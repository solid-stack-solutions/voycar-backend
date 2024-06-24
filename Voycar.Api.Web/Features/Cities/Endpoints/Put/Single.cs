namespace Voycar.Api.Web.Features.Cities.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<City>
{
    public Single(ICities repository) : base(repository, ["admin"]) {}
}
