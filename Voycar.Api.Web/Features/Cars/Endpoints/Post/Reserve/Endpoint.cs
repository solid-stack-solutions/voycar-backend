namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using Entities;
using Reservations.Repository;

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
        // ToDo remove before merging (useful for debugging)
        this.AllowAnonymous();
        //this.Roles(["admin", "employee", "member"]);
        this.Summary(s =>
        {
            s.Summary = $"Reserve {nameof(Car)}";
            s.Description = $"Reserve {nameof(Car)} for given member and timespan";
            s.Responses[200] = "Generated ID of created reservation";
            s.Responses[400] = "If end-time is not after begin-time or no station or member with the given ID exists";
            s.Responses[409] = $"If {nameof(Car)} could not be reserved due to conflicts with other reservations";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var reservationId = this._resRepository.Create(req.CarId, req.MemberId, req.Begin, req.End);

        if (reservationId is null)
        {
            await this.SendResultAsync(TypedResults.Conflict());
            return;
        }

        await this.SendResultAsync(TypedResults.Ok(reservationId));
    }
}
