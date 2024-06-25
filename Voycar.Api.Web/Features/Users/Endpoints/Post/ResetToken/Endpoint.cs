namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetToken;

using System.Security.Cryptography;
using Repository;
using Service;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IUsers _userRepository;
    private readonly ILogger<Endpoint> _logger;
    private readonly IEmailService _emailService;

    // User has 30 minutes to reset his password
    private const double ResetTokenValidTime = 30;

    public Endpoint(IUsers userRepository, ILogger<Endpoint> logger, IEmailService emailService)
    {
        this._userRepository = userRepository;
        this._logger = logger;
        this._emailService = emailService;
    }


    public override void Configure()
    {
        this.Post("auth/reset-token");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Checks whether there is a user for the request
        var user = await this._userRepository.RetrieveByEmail(req.Email.ToLowerInvariant());

        if (user is null)
        {
            this.ThrowError("User not found");
        }

        user.PasswordResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(256));
        user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(ResetTokenValidTime);

        this._userRepository.Update(user);

        this._emailService.SendPasswordResetEmail(user);
        this._logger.LogInformation("Password-Reset-Token successfully created for User with ID: {UserId}", user.Id);

        // ToDo PasswordResetToken must be removed later (is used for debug purposes)
        await this.SendResultAsync(TypedResults.Ok(
            new Response { PasswordResetToken = user.PasswordResetToken }
        ));
    }
}
