namespace Voycar.Api.Web.Features.Reservations.Endpoints.Delete;

using Repository;
using Entities;

public class Single : Generic.Endpoint.Delete.Single<Reservation>
{
    public Single(IReservations repository) : base(repository, ["admin"]) {}
}
