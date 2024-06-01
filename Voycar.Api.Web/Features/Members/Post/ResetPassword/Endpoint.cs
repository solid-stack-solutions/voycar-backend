namespace Voycar.Api.Web.Features.Members.Post.ResetPassword;

using System.Security.Cryptography;
using Repository;
using Services.EmailService;

// todo
public class Endpoint : Endpoint<Request>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<Get.Verify.Endpoint> _logger;

    public Endpoint(IMemberRepository memberRepository, ILogger<Get.Verify.Endpoint> logger)
    {
        this._memberRepository = memberRepository;
        this._logger = logger;
    }
    public override void Configure()
    {
        this.Post("/api/reset-password");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // checks whether there is a user for the req
        var user = await this._memberRepository.GetPrtAsync(req.PasswordResetToken);

        if (user is null || user.ResetTokenExpires < DateTime.Now)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // set new password hash
        user.PasswordHash =  BCrypt.Net.BCrypt.EnhancedHashPassword(req.Password);

        // remove reset options
        user.ResetTokenExpires = null;
        user.PasswordResetToken = null;

        await this._memberRepository.SaveAsync();


        this._logger.LogInformation("Password-Reset-Token successfully created for User with ID: {UserId}", user.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
