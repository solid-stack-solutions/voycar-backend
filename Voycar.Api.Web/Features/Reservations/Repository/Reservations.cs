namespace Voycar.Api.Web.Features.Reservations.Repository;

using Context;
using Entities;
using Microsoft.IdentityModel.Tokens;

public class Reservations : Generic.Repository.Repository<Reservation>, IReservations
{
    public Reservations(VoycarDbContext context) : base(context) {}

    public Guid? Create(Guid carId, Guid memberId, DateTime begin, DateTime end)
    {
        var noConflicts = this.DbSet
            // Reservations for the same car
            .Where(res => res.CarId == carId)
            // Reservations with conflicting timespans
            .Where(res =>
                // res.Begin is in given timespan
                (res.Begin >= begin && res.Begin < end) ||
                // res.End is in given timespan
                (res.End > begin && res.End <= end) ||
                // Whole given timespan overlaps with reservation
                (res.Begin < begin && res.End > end))
            .IsNullOrEmpty();

        if (!noConflicts)
        {
            return null;
        }

        return this.Create(new Reservation
        {
            Id = Guid.NewGuid(),
            Begin = begin,
            End = end,
            CarId = carId,
            MemberId = memberId
        });
    }
}
