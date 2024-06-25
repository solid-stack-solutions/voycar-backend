namespace Voycar.Api.Web.Features.Users.Endpoints.Post.ResetPassword;

using Repository;

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
        this.Post("auth/reset-password");
        this.AllowAnonymous();
        this.Summary(s =>
        {
            s.Summary = "Reset password of user";
            s.Description = "Reset password of user with reset token and new password";
            s.Responses[200] = "If reset is successful";
            s.Responses[400] = "If reset token or password are invalid or do not belong to a known user";
        });
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Checks whether there is a user for the request
        var user = await this._userRepository.RetrieveByPasswordResetToken(req.PasswordResetToken);

        if (user is null || user.ResetTokenExpires < DateTime.UtcNow)
        {
            this.ThrowError("Token does not belong to any user");
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
