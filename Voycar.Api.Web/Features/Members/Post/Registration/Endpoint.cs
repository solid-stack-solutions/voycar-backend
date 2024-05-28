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
    private readonly IMemberRepository memberRepository;
    private readonly IEmailService emailService;
    private readonly ILogger<Endpoint> logger;


    public Endpoint(IMemberRepository memberRepository, IEmailService emailService, ILogger<Endpoint> logger)
    {
        this.memberRepository = memberRepository;
        this.emailService = emailService;
        this.logger = logger;
    }
    public override void Configure()
    {
        this.Post("/api/registration");
        this.AllowAnonymous();
        this.logger.LogInformation("Registration endpoint configured.");
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        this.logger.LogInformation("Registration-Endpoint called.");
        // Check for existing user or password mismatch
        if (await this.memberRepository.GetAsync(req) is not null || req.Password != req.ConfirmPassword)
        {
            this.logger.LogWarning("User already exists or passwords do not match.");
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        this.logger.LogInformation("Creating new user.");
        var member = this.Map.ToEntity(req);
        await this.memberRepository.CreateAsync(member);
        this.logger.LogInformation("User created with ID: {MemberId}", member.Id);

        this.emailService.SendVerificationEmail(member);
        await this.SendAsync(new Response { VerificationToken = member.VerificationToken }, cancellation: ct);
    }
}
