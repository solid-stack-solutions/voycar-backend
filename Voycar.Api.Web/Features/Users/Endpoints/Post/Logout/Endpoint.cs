namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Logout;

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
