namespace Voycar.Api.Web.Features.Reservations.Endpoints.Put;

using Repository;
using Entities;

public class Single : Generic.Endpoint.Put.Single<Reservation>
{
    public Single(IReservations repository) : base(repository, ["admin", "employee"]) {}
}
