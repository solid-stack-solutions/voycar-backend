namespace Voycar.Api.Web.Features.Reservations.Endpoints.Get.Personal;

using Repository;
using Entities;
using Voycar.Api.Web.Features.Users.Repository;

public class Endpoint : Endpoint<Request, Results<Ok<Response>, BadRequest<string>>>
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
        this.Get(nameof(Reservation).ToLowerInvariant() + "/personal");
        this.Roles("member");
        this.Summary(summary =>
        {
            summary.Summary = $"Get {nameof(Reservation)}s of logged in member";
            summary.Description = $"Get the {nameof(Reservation)}s of the member who is logged in according to the request cookie";
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

        var reservations = this._reservationsRepository.RetrieveAll().Where(r => r.MemberId == user.MemberId).ToList();

        await this.SendResultAsync(TypedResults.Ok(FilterReservations(reservations)));
    }


    private static Response FilterReservations(List<Reservation> reservations)
    {
        return new Response
        {
            expired = reservations.Where(r => r.End < DateTime.UtcNow).ToList(),
            active = reservations.Where(r => r.Begin <= DateTime.UtcNow && r.End >= DateTime.UtcNow).ToList(),
            planned = reservations.Where(r => r.Begin > DateTime.UtcNow).ToList()
        };
    }
}
