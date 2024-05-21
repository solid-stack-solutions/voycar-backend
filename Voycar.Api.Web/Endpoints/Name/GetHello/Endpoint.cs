namespace Voycar.Api.Web.Endpoints.Name.GetHello;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(ILogger<Endpoint> logger)
    {
        this._logger = logger;
    }

    public override void Configure()
    {
        this._logger.LogInformation("Start setup");

        this.Post("/hello/world");
        this.AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        this._logger.LogInformation("Endpoint called");

        await this.SendAsync(new Response
        {
            FullName = $"{req.FirstName} {req.LastName}",
            Message = "Welcome to FastEndpoints..."
        }, cancellation: ct);
    }

}
