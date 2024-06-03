namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Repository;
using Services.EmailService;

/// <summary>
/// Handles the registration of new members.
///
/// This endpoint receives registration requests, checks for existing users,
/// creates new user accounts and sends verification emails.
/// </summary>
public class Endpoint : Endpoint<Request, Response, Mapper>
{
    private readonly IMembers members;
    private readonly IUsers _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<Endpoint> _logger;


    public Endpoint(IMembers members, IUsers userRepository, IEmailService emailService, ILogger<Endpoint> logger)
    {
        this.members = members;
        this._emailService = emailService;
        this._logger = logger;
        this._userRepository = userRepository;
    }
    public override void Configure()
    {
        this.Post("/api/registration");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (await this._userRepository.Retrieve(req.Email.ToLowerInvariant()) is not null)
        {
            this._logger.LogWarning("User already exists.");
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        this._logger.LogInformation("Creating new user.");
        var member = this.Map.ToEntity(req);
        this.members.Create(member);
        this._logger.LogInformation("User created with ID: {MemberId}", member.Id);

        this._emailService.SendVerificationEmail(member);
        await this.SendAsync(new Response { VerificationToken = member.VerificationToken }, cancellation: ct);
    }
}
