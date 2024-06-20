namespace Voycar.Api.Web.Features.Cars.Repository;

using Entities;

public interface ICars : Generic.Repository.IRepository<Car>
{
    /// <summary>
    /// Retrieve all <see cref="Car"/>s at the given <see cref="Station"/>
    /// that are not reserved (appear in no <see cref="Reservation"/>) during the whole given timespan.
    /// Assumes that <c>stationId</c> is valid and time between <c>begin</c> and <c>end</c> is positive.
    /// </summary>
    IEnumerable<Car> RetrieveAvailable(Guid stationId, DateTime begin, DateTime end);
}
