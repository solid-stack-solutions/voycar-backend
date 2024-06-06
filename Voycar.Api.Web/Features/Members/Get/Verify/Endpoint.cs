namespace Voycar.Api.Web.Features.Members.Get.Verify;

using Repository;

/// <summary>
/// Handles the verification of new members.
///
/// This endpoint receives verification tokens, checks their validity,
/// updates the member's verification status, and returns a response indicating success or failure.
/// </summary>
public class Endpoint : Endpoint<Request>
{
    private readonly IMembers _members;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IMembers members, ILogger<Endpoint> logger)
    {
        this._members = members;
        this._logger = logger;
    }
    public override void Configure()
    {
        this.Get("/verify/{verificationToken}");
        this.AllowAnonymous();
        Description(b => b
                .Accepts<Request>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(400),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = "GET verify Endpoint for Member";
            s.Description = "This Endpoint is used to verify a member against the database " +
                            "and update last time of verification";
            s.Responses[200] = "OK response if verification was successful";
            s.Responses[400] =
                "BAD REQUEST response if verification failed";
            s.ResponseExamples[400] = new {};
            s.Params["verificationToken"] = "The verification token of Member that has to be verified";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // checks whether there is a member for the token in order to verify it
        var member = this._members.Retrieve(req.VerificationToken);

        if (member is null)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        member.Result!.VerifiedAt = DateTime.UtcNow;
        this._members.Update(member.Result);

        this._logger.LogInformation("Member verified successfully with ID: {MemberId}", member.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
