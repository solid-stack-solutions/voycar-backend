namespace Voycar.Api.Web.Features.Members.Post.Login;

using Entities;
using FastEndpoints.Security;
using Repository;

public class Endpoint : Endpoint<Request, Results<Ok, BadRequest>>
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
        var user = await this._userRepository.Retrieve("email", req.Email.ToLowerInvariant());


        // Check if user entered valid credentials and verified his email
        if (user is null || user!.VerifiedAt is null ||
            !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, user.PasswordHash))
        {
            // Do not send different request for user not found, since this would reveal which users are signed up
            await this.SendResultAsync(TypedResults.BadRequest());
            return;
        }

        // Login member
        await this.SignInUserAsync(user!, ct);
    }


    private async Task SignInUserAsync(User user, CancellationToken ct)
    {
        var roleId = user.RoleId;
        if (roleId is null)
        {
            // Sign in without any roles
            await CookieAuth.SignInAsync(privileges => { });
        }
        else
        {
            var role = await this._members.RetrieveRole((Guid)roleId);
            await CookieAuth.SignInAsync(u =>
            {
                u.Roles.Add(role!.Name);
            });
        }

        this._logger.LogInformation("User logged in successfully with ID: {UserId}", user.Id);
        await this.SendResultAsync(TypedResults.Ok());
    }
}
