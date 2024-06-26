namespace Voycar.Api.Web.Features.Plans.Endpoints.Put.Personal;

using Entities;
using Members.Repository;
using Repository;
using Users.Repository;


public class Endpoint : Endpoint<Request, Ok>
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
        this.Put(nameof(Plan).ToLowerInvariant() + "/personal");
        this.Roles("member");
        this.Summary(summary =>
        {
            summary.Summary = "Update plan details of logged in member";
            summary.Description =
                "Update the plan details of the member who is logged in according to the request cookie";
            summary.Responses[200] = "If member is found and their plan is updated";
            summary.Responses[400] = "If the user somehow can't be found or the user is not a member " +
                                     "or their plan could not be updated";
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

        var plan = this._planRepository.Retrieve(req.PlanId);
        if (plan is null)
        {
            this.ThrowError("Plan does not exist");
        }

        var member = this._memberRepository.Retrieve(user.MemberId.Value);
        if (member is null)
        {
            this.ThrowError("Member does not exist");
        }

        if (plan.Id == member.PlanId)
        {
            this.ThrowError("Cannot update the plan with the same ID");
        }

        member.PlanId = plan.Id;
        if (!this._memberRepository.Update(member))
        {
            this.ThrowError("Failed to update member data");
        }

        await this.SendResultAsync(TypedResults.Ok());
    }
}
