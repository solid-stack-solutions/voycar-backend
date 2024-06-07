namespace Voycar.Api.Web.Features.Members.Post.Login;

using Entities;
using FastEndpoints.Security;
using Repository;


public class Endpoint : Endpoint<Request>
{
    private readonly IMembers _members;
    private readonly IUsers _userRepository;
    private readonly ILogger<Endpoint> _logger;


    public Endpoint(IMembers members, IUsers userRepository, ILogger<Endpoint> logger)
    {
        this._members = members;
        this._userRepository = userRepository;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Post("/login");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Member? member = null;

        var user = await this._userRepository.Retrieve("email", req.Email.ToLowerInvariant());

        // Check if user is a member (members must be verified)
        if (user is not null)
        {
            member = this._members.Retrieve(user.Id);
        }

        // Checks for employee / admin
        if (member is null)
        {
            // Check if employee or admin entered valid credentials
            if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, user.PasswordHash))
            {
                await this.SendErrorsAsync(cancellation: ct);
                return;
            }

            // Login employee / admin
            await this.SignInUserAsync(user!, ct);
            return;
        }

        // Check if member entered valid credentials and is verified
        if (member!.VerifiedAt is null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, member.User.PasswordHash))
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // Login member
        await this.SignInUserAsync(user!, ct);
    }


    private async Task SignInUserAsync(User user, CancellationToken ct)
    {
        var role = await this._members.RetrieveRole(user.RoleId);
        await CookieAuth.SignInAsync(u =>
        {
            u.Roles.Add(role!.Name);
        });
        this._logger.LogInformation("User logged in successfully with ID: {UserId}", user.Id);
        await this.SendOkAsync(cancellation: ct);
    }
}
