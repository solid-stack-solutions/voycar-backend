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
            summary.Responses[400] = "If one of the following occurs: user somehow can't be found, " +
                "user ist not a member, reservertion does not exist/could not be deleted";
        });
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = this._userRepository.Retrieve(req.UserId);
        if (user is null)
        {
            this.ThrowError("User does not exist");
        }

        if (user.MemberId is null)
        {
            this.ThrowError("Member does not exist");
        }


        var reservation = this._reservationsRepository.Retrieve(req.Id);
        if (reservation is null)
        {
            this.ThrowError("Reservation does not exist");
        }

        if (!(reservation.Begin > DateTime.UtcNow))
        {
            this.ThrowError("Reservation cannot be deleted");
        }

        if (!this._reservationsRepository.Delete(reservation.Id))
        {
            this.ThrowError("Failed to delete reservation");
        }

        await this.SendResultAsync(TypedResults.Ok());
    }
}
