namespace Voycar.Api.Web.Features.Reservation.Repository;

using Context;
using Entities;

public class Reservations : Generic.Repository.Repository<Reservation>, IReservations
{
    public Reservations(VoycarDbContext context) : base(context) {}
}
