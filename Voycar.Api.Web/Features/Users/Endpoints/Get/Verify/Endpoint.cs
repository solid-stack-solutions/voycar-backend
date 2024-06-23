namespace Voycar.Api.Web.Features.Users.Endpoints.Get.Verify;

using Repository;

/// <summary>
/// Handles the verification of new users.
///
/// This endpoint receives a verification token, checks the validity,
/// updates the user's verification status, and returns a response indicating success or failure.
/// </summary>
public class Endpoint : Endpoint<Request>
{
    private readonly IUsers _userRepository;
    private readonly ILogger<Endpoint> _logger;


    public Endpoint(IUsers userRepository, ILogger<Endpoint> logger)
    {
        this._userRepository = userRepository;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Get("auth/verify/{verificationToken}");
        this.AllowAnonymous();
        this.Summary(s =>
        {
            s.Summary = "Verify user";
            s.Description = "Verify a user against the database " +
                            "and update time of verification";
            s.Responses[200] = "If verification is successful";
            s.Responses[400] =
                "If verification fails";
            s.Params["verificationToken"] = "Verification token of the user to be verified";
        });
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Checks whether there is a user for the token in order to verify it
        var user = await this._userRepository.RetrieveByVerificationToken(req.VerificationToken);

        if (user is null)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        user.VerifiedAt = DateTime.UtcNow;
        this._userRepository.Update(user);

        this._logger.LogInformation("User verified successfully with ID: {UserId}", user.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
