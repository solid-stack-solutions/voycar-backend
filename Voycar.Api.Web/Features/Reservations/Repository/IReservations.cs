namespace Voycar.Api.Web.Features.Reservations.Repository;

using Entities;

public interface IReservations : Generic.Repository.IRepository<Reservation>
{
    /// <summary>
    /// Assumes that all attributes of the given <see cref="Reservation"/> are valid.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <see cref="Reservation"/> is conflicting with other
    /// reservations in the database (for the same car in the given timespan)
    /// </returns>
    bool HasConflicts(Reservation reservation);
}
