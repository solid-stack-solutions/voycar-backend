namespace Voycar.Api.Web.Features.Members.Post.Verify;

using Repository;


/// <summary>
/// Handles the verification of new members.
///
/// This endpoint receives verification tokens, checks their validity,
/// updates the member's verification status, and returns a response indicating success or failure.
/// </summary>
public class Endpoint : Endpoint<Request>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IMemberRepository memberRepository, ILogger<Endpoint> logger)
    {
        this._memberRepository = memberRepository;
        this._logger = logger;
    }
    public override void Configure()
    {
        this.Get("/api/verify/{verificationToken}");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // checks whether there is a member for the token in order to verify it
        var member = await this._memberRepository.GetAsync(req.VerificationToken);

        if (member is null)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        member.VerifiedAt = DateTime.UtcNow;
        await this._memberRepository.SaveAsync();

        this._logger.LogInformation("Member verified successfully with ID: {MemberId}", member.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
