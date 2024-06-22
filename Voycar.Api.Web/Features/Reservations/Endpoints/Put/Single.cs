namespace Voycar.Api.Web.Features.Reservations.Endpoints.Put;

using Repository;
using Entities;

public class Single : Generic.Endpoint.Put.Single<Reservation>
{
    // ToDo roles
    public Single(IReservations repository) : base(repository, ["admin"]) {}
}
