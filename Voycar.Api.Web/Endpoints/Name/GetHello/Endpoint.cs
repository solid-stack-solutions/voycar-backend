namespace Voycar.Api.Web.Endpoints.Name.GetHello;

public class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        this.Post("/hello/world");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await this.SendAsync(new Response
                {
                    FullName = $"{req.FirstName} {req.LastName}",
                    Message = "Welcome to FastEndpoints..."
                }, cancellation: ct);
    }

}
