namespace Voycar.Api.Web.Features.Name.GetHello;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ILogger<Endpoint> logger;

    public Endpoint(ILogger<Endpoint> logger)
    {
        this.logger = logger;
    }

    public override void Configure()
    {
        this.logger.LogInformation("Start setup");

        this.Post("/hello/world");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        this.logger.LogInformation("Endpoint called");

        await this.SendAsync(new Response
        {
            FullName = $"{req.FirstName} {req.LastName}",
            Message = "Welcome to FastEndpoints..."
        }, cancellation: ct);
    }

}
