namespace Voycar.Api.Web.Features.Reservations.Endpoints.Get;

using Repository;
using Entities;

public class Single : Generic.Endpoint.Get.Single<Reservation>
{
    public Single(IReservations repository) : base(repository, ["admin", "employee"]) {}
}
