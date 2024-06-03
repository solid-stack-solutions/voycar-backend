namespace Voycar.Api.Web.Features.Members.Post.Login;

using Entities;
using FastEndpoints.Security;
using Repository;

public class Endpoint : Endpoint<Request>
{
    private readonly IMembers members;
    private readonly IUsers _userRepository;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IMembers members, IUsers userRepository, ILogger<Endpoint> logger)
    {
        this.members = members;
        this._userRepository = userRepository;
        this._logger = logger;
    }
    public override void Configure()
    {
        this.Post("/api/login");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Member? member = null;

        var user = await this._userRepository.Retrieve(req.Email.ToLowerInvariant());

        // check if user is a member (members must be verified)
        if (user is not null)
        {
            member = this.members.Retrieve(user.Id);
        }

        // checks for employee / admin
        if (member is null)
        {
            // check if employee or admin entered valid credentials
            if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, user.PasswordHash))
            {
                await this.SendErrorsAsync(cancellation: ct);
                return;
            }

            // login employee / admin
            await this.SignInUserAsync(user!);
            this._logger.LogInformation("User logged successfully in with ID: {UserId}", user.Id);
            return;
        }


        // check if member entered valid credentials and is verified
        if (member!.VerifiedAt is null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, member.User.PasswordHash))
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // login member
        await this.SignInUserAsync(user!);
        this._logger.LogInformation("Member logged successfully in with ID: {MemberId}", member.Id);
        await this.SendOkAsync(cancellation: ct);
    }

    private async Task SignInUserAsync(User user)
    {
        var role = await this.members.RetrieveRole(user.RoleId);
        await CookieAuth.SignInAsync(u =>
        {
            u.Roles.Add(role!.Name);
        });
    }
}
