namespace Voycar.Api.Web.Features.Reservations.Repository;

using Context;
using Entities;

public class Reservations : Generic.Repository.Repository<Reservation>, IReservations
{
    public Reservations(VoycarDbContext context) : base(context) {}

    public Guid? Create(Guid carId, Guid memberId, DateTime begin, DateTime end)
    {
        return null;
    }
}
