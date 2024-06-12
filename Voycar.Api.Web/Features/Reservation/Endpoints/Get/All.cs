namespace Voycar.Api.Web.Features.Reservation.Endpoints.Get;

using Entities;
using Repository;

public class All : Generic.Endpoint.Get.All<Reservation>
{
    // ToDo roles
    public All(IReservations repository) : base(repository, ["admin"]) {}
}
