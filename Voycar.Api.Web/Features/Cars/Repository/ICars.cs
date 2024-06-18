namespace Voycar.Api.Web.Features.Cars.Repository;

using Entities;

public interface ICars : Generic.Repository.IRepository<Car>
{
    IEnumerable<Car> RetrieveAvailable(Guid stationId, DateTime begin, DateTime end);
}
