namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Repository;
using Services.EmailService;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    private readonly IMemberRepository memberRepository;
    private readonly IEmailService emailService;


    public Endpoint(IMemberRepository memberRepository, IEmailService emailService)
    {
        this.memberRepository = memberRepository;
        this.emailService = emailService;
    }
    public override void Configure()
    {
        this.Post("/api/registration");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (await this.memberRepository.GetAsync(req) is not null || req.Password != req.ConfirmPassword)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }

        var member = this.Map.ToEntity(req);
        await this.memberRepository.CreateAsync(member);

        this.emailService.SendVerificationEmail(member);
        await this.SendAsync(new Response { VerificationToken = member.VerificationToken }, cancellation: ct);
    }
}
