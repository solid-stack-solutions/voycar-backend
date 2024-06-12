namespace Voycar.Api.Web.Features.Reservation.Endpoints.Get;

using Entities;
using Repository;

public class Single : Generic.Endpoint.Get.Single<Reservation>
{
    // ToDo roles
    public Single(IReservations repository) : base(repository, ["admin"]) {}
}
