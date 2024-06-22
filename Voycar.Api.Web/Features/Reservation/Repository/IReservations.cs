namespace Voycar.Api.Web.Features.Reservation.Repository;

using Entities;

public interface IReservations : Generic.Repository.IRepository<Reservation>
{
    Guid? Create(Guid carId, Guid memberId, DateTime begin, DateTime end);
}
