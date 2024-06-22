namespace Voycar.Api.Web.Features.Reservations.Repository;

using Context;
using Entities;

public class Reservations : Generic.Repository.Repository<Reservation>, IReservations
{
    public Reservations(VoycarDbContext context) : base(context) {}
}
