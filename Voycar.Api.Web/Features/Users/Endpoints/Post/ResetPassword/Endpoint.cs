namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetPassword;

using Repository;

public class Endpoint : Endpoint<Request, Results<Ok, BadRequest<ErrorResponse>>>
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
        this.Post("auth/reset-password");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Checks whether there is a user for the request
        var user = await this._userRepository.Retrieve("passwordResetToken", req.PasswordResetToken);

        if (user is null || user.ResetTokenExpires < DateTime.UtcNow)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // Set new password hash
        user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(req.Password);

        // Remove reset options
        user.ResetTokenExpires = null;
        user.PasswordResetToken = null;

        this._userRepository.Update(user);

        this._logger.LogInformation("Password successfully reset for User with ID: {UserId}", user.Id);
        await this.SendResultAsync(TypedResults.Ok());
    }
}
