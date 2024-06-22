namespace Voycar.Api.Web.Features.Reservations.Repository;

using Context;
using Entities;
using Microsoft.IdentityModel.Tokens;

public class Reservations : Generic.Repository.Repository<Reservation>, IReservations
{
    public Reservations(VoycarDbContext context) : base(context) {}

    public bool HasConflicts(Reservation reservation)
    {
        return !this.DbSet
            // Reservations for the same car
            .Where(res => res.CarId == reservation.CarId)
            // Reservations with conflicting timespans
            .Where(res =>
                // res.Begin is in given timespan
                (res.Begin >= reservation.Begin && res.Begin <  reservation.End) ||
                // res.End is in given timespan
                (res.End   >  reservation.Begin && res.End   <= reservation.End) ||
                // Whole given timespan overlaps with reservation
                (res.Begin <  reservation.Begin && res.End   >  reservation.End))
            .IsNullOrEmpty();
    }
}
