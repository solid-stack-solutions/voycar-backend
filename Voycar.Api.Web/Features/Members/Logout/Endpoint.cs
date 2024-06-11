namespace Voycar.Api.Web.Features.Members.Logout;

using FastEndpoints.Security;


public class Endpoint : EndpointWithoutRequest<Ok>
{
    public override void Configure()
    {
        this.Post("auth/logout");
        this.Roles("admin", "employee", "member");
    }


    public override async Task HandleAsync(CancellationToken ct)
    {
        await CookieAuth.SignOutAsync();
        await this.SendOkAsync(cancellation: ct);
    }
}
