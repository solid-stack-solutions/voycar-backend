namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Register;

using System.Security.Cryptography;
using Entities;
using Plans.Repository;
using Repository;
using Service;


/// <summary>
/// Handles the registration of new members.
///
/// This endpoint receives a registration request, checks for existing users,
/// creates a new user account with a member entity and sends a verification email.
/// </summary>
public class Endpoint : Endpoint<Request, Ok, Mapper>
{
    private readonly IUsers _userRepository;
    private readonly Members.Repository.IMembers _memberRepository;
    private readonly Roles.Repository.IRoles _roleRepository;
    private readonly IPlans _planRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<Endpoint> _logger;

    private const string MemberRoleName = "member";


    public Endpoint(IUsers userRepository, Members.Repository.IMembers memberRepository,
        Roles.Repository.IRoles roleRepository, IPlans planRepository,
        IEmailService emailService, ILogger<Endpoint> logger)
    {
        this._userRepository = userRepository;
        this._memberRepository = memberRepository;
        this._roleRepository = roleRepository;
        this._planRepository = planRepository;
        this._emailService = emailService;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Post("auth/register");
        this.AllowAnonymous();
        this.Summary(s =>
        {
            s.Summary = "Register new member";
            s.Description = "Register a new member (if they don't exist already) and send a verification email";
            s.Responses[200] = "If registration is successful and verification email was sent";
            s.Responses[400] = "If member already exists or plan ID or personal details are invalid";
        });
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Check for existing users
        if (await this._userRepository.RetrieveByEmail(req.Email.ToLowerInvariant()) is not null)
        {
            this._logger.LogWarning("User {UserMail} already exists.", req.Email);
            this.ThrowError("User already exists");
        }

        if (this._planRepository.Retrieve(req.PlanId) is null)
        {
            this._logger.LogWarning("PlanId {PlanId} does not exist.", req.PlanId);
            this.ThrowError("Plan does not exist");
        }

        this._logger.LogInformation("Creating new member and user entities.");
        // Create member and user as local variables before saving them in DB to avoid conflicts in DB
        var member = this.Map.ToEntity(req);
        var user = this.CreateUser(req, member);

        this._memberRepository.Create(member);
        this._logger.LogInformation("User created with Email: {UserEmail}", user.Email);
        this._userRepository.Create(user);
        this._logger.LogInformation("Member created with ID: {MemberId}", member.Id);


        this._emailService.SendVerificationEmail(user);
        await this.SendResultAsync(TypedResults.Ok());
    }


    private User CreateUser(Request req, Member member)
    {
        return new User
        {
            Email = req.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(req.Password),
            VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(256)),
            // Retrieve member role
            RoleId = this._roleRepository.Retrieve(MemberRoleName).Result!.Id,
            MemberId = member.Id
        };
    }
}
