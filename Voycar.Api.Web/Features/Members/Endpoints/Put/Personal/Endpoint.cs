namespace Voycar.Api.Web.Features.Members.Endpoints.Put.Personal;

using Entities;
using Plans.Repository;
using Repository;
using Users.Repository;


public class Endpoint : Endpoint<Request, Ok, Mapper>
{
    private readonly IUsers _userRepository;
    private readonly IMembers _memberRepository;
    private readonly IPlans _planRepository;


    public Endpoint(IUsers userRepository, IMembers memberRepository, IPlans planRepository)
    {
        this._userRepository = userRepository;
        this._memberRepository = memberRepository;
        this._planRepository = planRepository;
    }


    public override void Configure()
    {
        this.Put(nameof(Member).ToLowerInvariant() + "/personal");
        this.Roles("member");
        this.Summary(summary =>
        {
            summary.Summary = "Update personal details of logged in member";
            summary.Description =
                "Update the personal details of the member who is logged in according to the request cookie";
            summary.Responses[200] = "If member is found and updated";
            summary.Responses[400] = "If the user somehow can't be found or the user is not a member " +
                                     "or member could not be updated";
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
            this.ThrowError("User is not a member");
        }

        if (this._planRepository.Retrieve(req.PlanId) is null)
        {
            this.ThrowError("Plan does not exist");
        }

        var member = this._memberRepository.Retrieve(user.MemberId.Value);
        if (member is null)
        {
            this.ThrowError("Member does not exist");
        }

        if (!this._memberRepository.Update(this.Map.ToEntity(req, member)))
        {
            this.ThrowError("Failed to update member data");
        }


        await this.SendResultAsync(TypedResults.Ok());
    }
}
