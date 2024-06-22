namespace Voycar.Api.Web.Features.Members.Endpoints.Put.Personal;

using Entities;
using FluentValidation.Results;
using Repository;
using Users.Repository;


public class Endpoint : Endpoint<Request, Results<Ok, BadRequest<string>>, Mapper>
{
    private readonly IUsers _userRepository;
    private readonly IMembers _memberRepository;


    public Endpoint(IUsers userRepository, IMembers memberRepository)
    {
        this._userRepository = userRepository;
        this._memberRepository = memberRepository;
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
            summary.Responses[200] = "If member is found";
            summary.Responses[400] = "If the user somehow can't be found or the user is not a member";
        });
        this.DontThrowIfValidationFails();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (this.ValidationFailed)
        {
            var errorMessages = new List<string>();
            foreach (var failure in this.ValidationFailures)
            {
                errorMessages.Add(failure.ErrorMessage);
            }

            await this.SendResultAsync(TypedResults.BadRequest(errorMessages));
        }

        var user = this._userRepository.Retrieve(req.UserId);
        if (user is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("User does not exist"));
            return;
        }

        if (user.MemberId is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("User is not a member"));
            return;
        }


        var member = this._memberRepository.Retrieve(user.MemberId.Value);
        if (member is null)
        {
            await this.SendResultAsync(TypedResults.BadRequest("Member does not exist"));
            return;
        }

        if (!this._memberRepository.Update(this.Map.ToEntity(req, member)))
        {
            await this.SendResultAsync(TypedResults.BadRequest("Failed to update member data"));
            return;
        }

        await this.SendResultAsync(TypedResults.Ok());
    }
}
