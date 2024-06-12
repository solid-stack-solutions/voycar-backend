namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Login;

using Entities;
using Repository;

public class Endpoint : Endpoint<Request, Results<Ok, BadRequest<ErrorResponse>>>
{
    private readonly IUsers _userRepository;
    private readonly Roles.Repository.IRoles _rolesRepository;
    private readonly ILogger<Endpoint> _logger;


    public Endpoint(IUsers userRepository, Roles.Repository.IRoles rolesRepository, ILogger<Endpoint> logger)
    {
        this._userRepository = userRepository;
        this._rolesRepository = rolesRepository;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Post("auth/login");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await this._userRepository.Retrieve("email", req.Email!.ToLowerInvariant());


        // Check if user entered valid credentials and verified his email
        if (user is null || user!.VerifiedAt is null ||
            !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, user.PasswordHash))
        {
            // Do not send different request for user not found, since this would reveal which users are signed up
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // Login member
        await this.SignInUserAsync(user!);
    }


    /// <summary>
    /// Signs in a user with cookie authentication and assigns any possible roles.
    /// If any roles are associated with that user the roles are added to the cookie. A response including the
    /// cookie is sent back.
    /// </summary>
    /// <param name="user">The user to sign in</param>
    private async Task SignInUserAsync(User user)
    {
        var role = this._rolesRepository.Retrieve(user.RoleId);
        await CookieAuth.SignInAsync(privileges =>
        {
            privileges.Roles.Add(role!.Name);
        });

        this._logger.LogInformation("User logged in successfully with ID: {UserId}", user.Id);
        await this.SendResultAsync(TypedResults.Ok());
    }
}
