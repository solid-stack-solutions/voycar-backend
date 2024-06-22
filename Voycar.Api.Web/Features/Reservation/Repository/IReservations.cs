namespace Voycar.Api.Web.Features.Reservation.Repository;

using Entities;

public interface IReservations : Generic.Repository.IRepository<Reservation>
{
    /// <summary>
    /// Create a new <see cref="Reservation"/> after checking for conflicts.
    /// Assumes that <c>carId</c> and <c>memberId</c> are valid
    /// and time between <c>begin</c> and <c>end</c> is positive.
    /// </summary>
    /// <returns>
    /// <c>null</c> if <see cref="Reservation"/> was not created due to conflicts.
    /// Otherwise, generated ID of created <see cref="Reservation"/>.
    /// </returns>
    Guid? Create(Guid carId, Guid memberId, DateTime begin, DateTime end);
}
