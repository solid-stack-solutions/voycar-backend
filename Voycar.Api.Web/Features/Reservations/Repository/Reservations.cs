namespace Voycar.Api.Web.Features.Reservations.Repository;

using Voycar.Api.Web.Context;
using Voycar.Api.Web.Entities;

public class Reservations : Generic.Repository.Repository<Reservation>, IReservations
{
    public Reservations(VoycarDbContext context) : base(context) {}
}
