namespace Voycar.Api.Web.Features.Reservations.Endpoints.Post;

using Repository;
using Entities;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Reservation>
{
    public SingleUnique(IReservations repository) : base(repository, ["admin", "employee"]) {}
}
