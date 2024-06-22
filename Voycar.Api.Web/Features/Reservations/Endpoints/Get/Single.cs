namespace Voycar.Api.Web.Features.Reservations.Endpoints.Get;

using Repository;
using Entities;

public class Single : Generic.Endpoint.Get.Single<Reservation>
{
    // ToDo roles
    public Single(IReservations repository) : base(repository, ["admin"]) {}
}
