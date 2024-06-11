namespace Voycar.Api.Web.Features.Cities.Repository;

using Context;
using Entities;

public class Cities : Generic.Repository.Repository<City>, ICities
{
    public Cities(VoycarDbContext context) : base(context) {}
}
