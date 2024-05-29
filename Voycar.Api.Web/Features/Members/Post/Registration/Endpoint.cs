namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Repository;
using Services.EmailService;

/// <summary>
/// Handles the registration of new members.
///
/// This endpoint receives registration requests, checks for existing users or password mismatches,
/// creates new user accounts and sends verification emails
/// </summary>
public class Endpoint : Endpoint<Request, Response, Mapper>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<Endpoint> _logger;


    public Endpoint(IMemberRepository memberRepository, IEmailService emailService, ILogger<Endpoint> logger)
    {
        this._memberRepository = memberRepository;
        this._emailService = emailService;
        this._logger = logger;
    }
    public override void Configure()
    {
        this.Post("/api/registration");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (await this._memberRepository.GetAsync(req) is not null || req.Password != req.ConfirmPassword)
        {
            this._logger.LogWarning("User already exists or passwords do not match.");
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        this._logger.LogInformation("Creating new user.");
        var member = this.Map.ToEntity(req);
        await this._memberRepository.CreateAsync(member);
        this._logger.LogInformation("User created with ID: {MemberId}", member.Id);

        this._emailService.SendVerificationEmail(member);
        await this.SendAsync(new Response { VerificationToken = member.VerificationToken }, cancellation: ct);
    }
}
