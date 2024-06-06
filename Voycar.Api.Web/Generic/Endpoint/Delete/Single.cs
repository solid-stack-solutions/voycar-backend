namespace Voycar.Api.Web.Generic.Endpoint.Delete;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<Entity, Results<Ok, NotFound>>
    where TEntity : Entity
{
    private readonly IRepository<TEntity> _repository;
    private readonly string[] roles;

    public Single(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.roles = roles;
    }

    public override void Configure()
    {
        this.Delete(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.roles);
        Description(b => b
                .Accepts<Entity>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(404),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = $"DELETE Endpoint for {typeof(TEntity).Name}";
            s.Description = $"This Endpoint is used to remove {typeof(TEntity).Name} objects from the database";
            s.Responses[200] = "OK response if DELETE operation was successful";
            s.Responses[404] =
                "Not Found response if DELETE operation was performed for on an Entity that could not be found in the database";
            s.ResponseExamples[404] = new {};
            s.Params["id"] = $"id derived from the {typeof(TEntity).Name} object";
        });
    }

    public override async Task HandleAsync(Entity req, CancellationToken ct)
    {
        var deleted = this._repository.Delete(req.Id);

        if (deleted)
        {
            await this.SendResultAsync(TypedResults.Ok());
            return;
        }

        await this.SendResultAsync(TypedResults.NotFound());
    }
}
