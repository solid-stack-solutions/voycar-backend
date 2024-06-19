namespace Voycar.Api.Web.Features.Cars.Repository;

using Context;
using Entities;

public class Cars : Generic.Repository.Repository<Car>, ICars
{
    private readonly ILogger<Endpoint> _logger;

    public Cars(VoycarDbContext context, ILogger<Endpoint> logger) : base(context)
    {
        this._logger = logger;
    }

    public IEnumerable<Car> RetrieveAvailable(Guid stationId, DateTime begin, DateTime end)
    {
        this._logger.LogDebug($"Timespan: {end - begin}");
        // ToDo
        return [];
    }
}
