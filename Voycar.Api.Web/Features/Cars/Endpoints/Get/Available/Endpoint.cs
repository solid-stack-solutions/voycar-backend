namespace Voycar.Api.Web.Features.Cars.Endpoints.Get.Available;

using Entities;
using Repository;

public class Endpoint : Endpoint<Request, IEnumerable<Car>>
{
    private readonly ICars _repository;

    public Endpoint(ICars repository)
    {
        this._repository = repository;
    }

    public override void Configure()
    {
        this.Get(nameof(Car).ToLowerInvariant() + "/available");
        // ToDo remove before merging (useful for debugging)
        this.AllowAnonymous();
        // ToDo roles
        //this.Roles(["admin"]);
        this.Summary(s =>
        {
            s.Summary = $"Retrieve available {nameof(Car)}s";
            s.Description = $"Retrieve {nameof(Car)} objects from the database at the given " +
                            "station that are available (not reserved) in the given timespan";
            s.Responses[200] = $"Available {nameof(Car)} objects (may be an empty array)";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await this.SendResultAsync(TypedResults.Ok(
            this._repository.RetrieveAvailable(req.StationId, req.Begin, req.End)));
    }
}
