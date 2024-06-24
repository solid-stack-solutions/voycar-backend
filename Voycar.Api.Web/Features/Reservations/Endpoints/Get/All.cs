namespace Voycar.Api.Web.Features.Reservations.Endpoints.Get;

using Repository;
using Entities;

public class All : Generic.Endpoint.Get.All<Reservation>
{
    public All(IReservations repository) : base(repository, ["admin", "employee"]) {}
}
