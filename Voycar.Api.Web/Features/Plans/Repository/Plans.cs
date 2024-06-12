namespace Voycar.Api.Web.Features.Plans.Repository;

using Context;
using Entities;

public class Plans : Generic.Repository.Repository<Plan>, IPlans
{
    public Plans(VoycarDbContext context) : base(context) {}
}
