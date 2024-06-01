namespace Voycar.Api.Web.Features.Members.Post.Login;

using Entities;
using FastEndpoints.Security;
using Repository;

public class Endpoint : Endpoint<Request>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IMemberRepository memberRepository, ILogger<Endpoint> logger)
    {
        this._memberRepository = memberRepository;
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
        // checks whether there is a member for the token in order to verify it
        var user = await this._memberRepository.GetAsync(req);

        // check if user is a member (members must be verified)
        if (user is not null)
        {
            member = await this._memberRepository.GetAsync(user.Id);
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


        // check if member entered valid credentials
        if (member!.VerifiedAt is null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, member.User.PasswordHash))
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        // login member
        await this.SignInUserAsync(user!);
        this._logger.LogInformation("Member logged successfully in with ID: {MemberId}", member.UserId);
        await this.SendOkAsync(cancellation: ct);
    }

    private async Task SignInUserAsync(User user)
    {
        var role = await this._memberRepository.GetRoleAsync(user.RoleId);
        await CookieAuth.SignInAsync(u =>
        {
            u.Roles.Add(role!.ToString()!);
        });
    }
}
