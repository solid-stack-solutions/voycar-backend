namespace Voycar.Api.Web.Features.Reservation.Endpoints.Delete;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Delete.Single<Reservation>
{
    // ToDo roles
    public Single(IReservations repository) : base(repository, ["admin"]) {}
}
