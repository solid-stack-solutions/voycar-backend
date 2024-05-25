namespace Voycar.Api.Web.Features.Members.Post.Registration;

using Repository;
using Services.EmailService;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    private readonly IMemberRepository memberRepository;
    private readonly IEmailSender emailSender;


    public Endpoint(IMemberRepository memberRepository, IEmailSender emailSender)
    {
        this.memberRepository = memberRepository;
        this.emailSender = emailSender;
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

        await this.SendAsync(new Response(), cancellation: ct);
    }
}
