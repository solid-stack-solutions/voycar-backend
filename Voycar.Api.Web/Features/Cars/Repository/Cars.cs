namespace Voycar.Api.Web.Features.Cars.Repository;

using Context;
using Entities;

public class Cars : Generic.Repository.Repository<Car>, ICars
{
    public Cars(VoycarDbContext context) : base(context) {}
}
