namespace Voycar.Api.Web.Features.Members.Post.Verify;

using Repository;
using Services.EmailService;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IMemberRepository memberRepository;

    public Endpoint(IMemberRepository memberRepository)
    {
        this.memberRepository = memberRepository;
    }
    public override void Configure()
    {
        this.Get("/api/verify/{verificationToken}");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var member = await this.memberRepository.GetAsync(req.VerificationToken);
        if (member is null)
        {
            await this.SendErrorsAsync(cancellation: ct);
            return;
        }
        member.VerifiedAt = DateTime.UtcNow;
        await this.memberRepository.SafeAsync();
        await this.SendOkAsync(cancellation: ct);
    }

}
