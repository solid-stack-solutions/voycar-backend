namespace Voycar.Api.Web.Features.Stations.Repository;

using Context;
using Entities;

public class Stations : Generic.Repository.Repository<Station>, IStations
{
    public Stations(VoycarDbContext context) : base(context) {}
}
