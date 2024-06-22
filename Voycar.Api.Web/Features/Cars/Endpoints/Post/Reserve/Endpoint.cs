namespace Voycar.Api.Web.Features.Cars.Endpoints.Post.Reserve;

using Entities;
using Generic;
using Members.Repository;
using Reservations.Repository;
using Users.Repository;

public class Endpoint : Endpoint<Request, Results<Ok<Entity>, Conflict>>
{
    private readonly IReservations _resRepository;
    private readonly IUsers _userRepository;
    private readonly IMembers _memberRepository;

    public Endpoint(IReservations resRepository, IUsers userRepository, IMembers memberRepository)
    {
        this._resRepository = resRepository;
        this._userRepository = userRepository;
        this._memberRepository = memberRepository;
    }

    public override void Configure()
    {
        this.Post(nameof(Car).ToLowerInvariant() + "/reserve");
        this.Roles("member");
        this.Summary(s =>
        {
            s.Summary = $"Reserve {nameof(Car)}";
            s.Description = $"Reserve {nameof(Car)} for logged in member and given timespan";
            s.Responses[200] = "Generated ID of created reservation";
            s.Responses[400] = "If end-time is not after begin-time or user is not a member or "
                             + "no station or user or member with the given ID exists";
            s.Responses[409] = $"If {nameof(Car)} could not be reserved due to conflicts with other reservations";
            s.ResponseExamples = new Dictionary<int, object> {{ 200, new Entity { Id = Guid.NewGuid() }}};
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = this._userRepository.Retrieve(req.UserId)!;
        if (user.MemberId is null)
        {
            this.ThrowError("User is not a member");
        }
        var memberId = user.MemberId.Value;
        if (this._memberRepository.Retrieve(memberId) is null)
        {
            this.ThrowError("Member does not exist");
        }

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            Begin = req.Begin,
            End = req.End,
            MemberId = memberId,
            CarId = req.CarId
        };

        if (this._resRepository.HasConflicts(reservation))
        {
            await this.SendResultAsync(TypedResults.Conflict());
            return;
        }

        var guid = this._resRepository.Create(reservation);
        await this.SendResultAsync(TypedResults.Ok(new Entity { Id = guid!.Value }));
    }
}
