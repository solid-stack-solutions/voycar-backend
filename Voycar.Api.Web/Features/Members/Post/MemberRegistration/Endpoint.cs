namespace Voycar.Api.Web.Features.Members.Post.MemberRegistration;

using Repository;
using Roles.Repository;

public class Endpoint : Endpoint<Request, Results<Ok, BadRequest>, Mapper>
{
    private readonly IUsers _userRepository;
    private readonly IMembers _memberRepository;
    private readonly IRoles _roleRepository;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IUsers userRepository, IMembers memberRepository, IRoles roleRepository, ILogger<Endpoint> logger)
    {
        this._userRepository = userRepository;
        this._memberRepository = memberRepository;
        this._roleRepository = roleRepository;
        this._logger = logger;
    }


    public override void Configure()
    {
        this.Post("auth/memberregister");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        // Verify which user is making the request; manually verify since this endpoint is accessible before logging in
        var user = await this._userRepository.Retrieve("email", req.Email.ToLowerInvariant());

        if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(req.Password, user.PasswordHash))
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        this._logger.LogInformation("Creating new member.");
        var member = this.Map.ToEntity(req);

        // Create member and save the member guid as foreign key in user
        user.MemberId = this._memberRepository.Create(member);

        // Retrieve the RoleId for the role named "member" and save in user
        user.RoleId = this._roleRepository.Retrieve("member").Result!.Id;

        this._userRepository.Update(user);
        this._logger.LogInformation("Member created with ID: {MemberId}", user.MemberId);

    }
}
