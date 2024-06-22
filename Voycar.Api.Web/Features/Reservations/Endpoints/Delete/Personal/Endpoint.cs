namespace Voycar.Api.Web.Features.Reservations.Endpoints.Delete.Personal;

using Entities;
using Repository;
using Users.Repository;


public class Endpoint : Endpoint<Request, Results<Ok, BadRequest<string>>>
{
    private readonly IUsers _userRepository;
    private readonly IReservations _reservationsRepository;


    public Endpoint(IUsers userRepository, IReservations reservationsRepository)
    {
        this._userRepository = userRepository;
        this._reservationsRepository = reservationsRepository;
    }


    public override void Configure()
    {
        this.Delete(nameof(Reservation).ToLowerInvariant() + "/personal/{id}");
        this.Roles("member");
        this.Summary(summary =>
        {
            summary.Summary = $"Delete {nameof(Reservation)} of logged in member";
            summary.Description =
                $"Delete the {nameof(Reservation)} of the member who is logged in according to the request cookie";
            summary.Responses[200] = "If member is found";
            summary.Responses[400] = "If the user somehow can't be found or the user is not a member.";
        });
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = this._userRepository.Retrieve(req.UserId);
        if (user is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("User does not exist"));
            return;
        }

        if (user.MemberId is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("Member does not exist"));
            return;
        }

        var reservation = this._reservationsRepository.Retrieve(req.Id);
        if (reservation is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("Reservation does not exist"));
            return;
        }

        if (!(reservation.Begin > DateTime.UtcNow))
        {
            await this.SendResultAsync(TypedResults.BadRequest("Reservation cannot be deleted"));
            return;
        }

        if (!this._reservationsRepository.Delete(reservation.Id))
        {
            await this.SendResultAsync(TypedResults.BadRequest("Failed to delete reservation"));
            return;
        }

        await this.SendResultAsync(TypedResults.Ok());
    }
}
