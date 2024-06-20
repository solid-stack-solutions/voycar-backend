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
        // Cars at given station
        var availableCars = this.RetrieveAll()
            .Where(car => car.StationId == stationId);

        // Reservations concerning availableCars conflicting with the given timespan
        var conflictingReservations = this._context.Reservations.ToArray()
            .Where(res => availableCars.Contains(res.Car))
            .Where(res =>
                // res.Begin is in given timespan
                (res.Begin >= begin && res.Begin <  end) ||
                // res.End is in given timespan
                (res.End   >  begin && res.End   <= end) ||
                // Whole given timespan overlaps with reservation
                (res.Begin <  begin && res.End   >  end));

        // availableCars with no conflicting reservations
        return availableCars.Where(car => conflictingReservations.All(res => car.Id != res.CarId));
    }
}
