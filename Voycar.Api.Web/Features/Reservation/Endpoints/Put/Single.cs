namespace Voycar.Api.Web.Features.Reservation.Endpoints.Put;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Put.Single<Reservation>
{
    // ToDo roles
    public Single(IReservations repository) : base(repository, ["admin"]) {}
}
