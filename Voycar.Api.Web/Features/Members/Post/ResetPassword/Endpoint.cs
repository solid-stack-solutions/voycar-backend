namespace Voycar.Api.Web.Features.Members.Post.ResetPassword;

using Repository;


public class Endpoint : Endpoint<Request>
{
    private readonly IUsers _repository;
    private readonly ILogger<Get.Verify.Endpoint> _logger;


    public Endpoint(IUsers repository, ILogger<Get.Verify.Endpoint> logger)
    {
        this._repository = repository;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Post("/reset-password");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Checks whether there is a user for the req
        var user = await this._repository.Retrieve("passwordResetToken", req.PasswordResetToken);

        if (user is null || user.ResetTokenExpires < DateTime.Now)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // Set new password hash
        user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(req.Password);

        // Remove reset options
        user.ResetTokenExpires = null;
        user.PasswordResetToken = null;

        this._repository.Update(user);

        this._logger.LogInformation("Password-Reset-Token successfully created for User with ID: {UserId}", user.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
