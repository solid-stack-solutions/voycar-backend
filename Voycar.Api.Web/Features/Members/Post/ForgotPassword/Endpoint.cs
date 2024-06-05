namespace Voycar.Api.Web.Features.Members.Post.ForgotPassword;

using System.Security.Cryptography;
using Repository;
using Services.EmailService;

public class Endpoint : Endpoint<Request>
{
    private readonly IUsers _repository;
    private readonly ILogger<Get.Verify.Endpoint> _logger;
    private readonly IEmailService _emailService;

    public Endpoint(IUsers repository, ILogger<Get.Verify.Endpoint> logger, IEmailService emailService)
    {
        this._repository = repository;
        this._logger = logger;
        this._emailService = emailService;
    }
    public override void Configure()
    {
        this.Post("/forgot-password");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // checks whether there is a user for the req
        var user = await this._repository.Retrieve(req.Email);

        if (user is null)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        user.PasswordResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(256));

        // user has 1 day to reset his password
        user.ResetTokenExpires = DateTime.UtcNow.AddDays(1);
        this._repository.Update(user);

        this._emailService.SendPasswordResetEmail(user);
        this._logger.LogInformation("Password-Reset-Token successfully created for User with ID: {UserId}", user.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
