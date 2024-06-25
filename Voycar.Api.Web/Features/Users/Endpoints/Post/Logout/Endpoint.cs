namespace Voycar.Api.Web.Features.Users.Endpoints.Post.Logout;

public class Endpoint : EndpointWithoutRequest<Ok>
{
    public override void Configure()
    {
        this.Post("auth/logout");
        this.Roles("admin", "employee", "member");
        this.Summary(s =>
        {
            s.Summary = "Logout user";
            s.Description = "Logout a user by withdrawing their cookie";
            s.Responses[200] = "If logout was attempted";
        });
    }


    public override async Task HandleAsync(CancellationToken ct)
    {
        await CookieAuth.SignOutAsync();
        await this.SendOkAsync(cancellation: ct);
    }
}
