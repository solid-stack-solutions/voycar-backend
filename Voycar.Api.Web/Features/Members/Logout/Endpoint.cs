namespace Voycar.Api.Web.Features.Members.Logout;

using FastEndpoints.Security;


public class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        this.Post("auth/logout");
        this.AllowAnonymous();
    }


    public override async Task HandleAsync(CancellationToken ct)
    {
        await CookieAuth.SignOutAsync();
        await this.SendOkAsync(cancellation: ct);
    }
}
