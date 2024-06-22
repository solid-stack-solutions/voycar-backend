namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using Entities;
using Reservation.Repository;

public class Endpoint : Endpoint<Request, Results<Ok<Guid>, Conflict>>
{
    private readonly IReservations _resRepository;

    public Endpoint(IReservations resRepository)
    {
        this._resRepository = resRepository;
    }

    public override void Configure()
    {
        this.Get(nameof(Car).ToLowerInvariant() + "/reserve");
        this.Roles(["admin", "employee", "member"]);
        this.Summary(s =>
        {
            s.Summary = $"Reserve {nameof(Car)}";
            s.Description = $"Reserve {nameof(Car)} for given member and timespan";
            s.Responses[200] = "Generated ID of created reservation";
            // ToDo other status codes
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await this.SendResultAsync(TypedResults.Ok(
            this._resRepository.Create(req.CarId, req.MemberId, req.Begin, req.End)));
    }
}
