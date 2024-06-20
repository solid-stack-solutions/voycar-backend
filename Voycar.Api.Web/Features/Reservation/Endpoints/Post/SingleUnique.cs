namespace Voycar.Api.Web.Features.Reservation.Endpoints.Post;

using Entities;
using Repository;

public class SingleUnique : Generic.Endpoint.Post.SingleUnique<Reservation>
{
    // ToDo roles
    public SingleUnique(IReservations repository) : base(repository, ["admin"]) {}
}
