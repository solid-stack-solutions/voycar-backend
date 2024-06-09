namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Repository;
using Services.EmailService;


/// <summary>
/// Handles the registration of new users.
///
/// This endpoint receives a registration request, checks for existing users,
/// creates a new user account and sends a verification mail.
/// </summary>
public class Endpoint : Endpoint<Request, Results<Ok<Response>, BadRequest>, Mapper>
{
    private readonly IUsers _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<Endpoint> _logger;


    public Endpoint(IUsers userRepository, IEmailService emailService, ILogger<Endpoint> logger)
    {
        this._userRepository = userRepository;
        this._emailService = emailService;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Post("auth/register");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (await this._userRepository.Retrieve("email", req.Email.ToLowerInvariant()) is not null)
        {
            this._logger.LogWarning("User {UserEmail} already exists.", req.Email.ToLowerInvariant());
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        this._logger.LogInformation("Creating new user.");
        var user = this.Map.ToEntity(req);
        this._userRepository.Create(user);
        this._logger.LogInformation("User created with ID: {MemberId}", user.Id);

        this._emailService.SendVerificationEmail(user);

        // ToDo VerificationToken must be removed later (is used for debug purposes)
        await this.SendAsync(TypedResults.Ok(new Response()
        {
            VerificationToken = user.VerificationToken
        }), cancellation: ct);
    }
}
