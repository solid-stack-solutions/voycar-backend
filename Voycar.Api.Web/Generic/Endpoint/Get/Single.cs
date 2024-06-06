namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<Entity, Results<Ok<TEntity>, NotFound>>
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
        this.Get(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.roles);
        Description(b => b
                .Accepts<Entity>("Voycar.Api.Web/Generic/Entity")
                .Produces<TEntity>(200)
                .ProducesProblem(404),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = $"GET Endpoint for {typeof(TEntity).Name}";
            s.Description =
                $"This Endpoint is used to retrieve a {typeof(TEntity).Name} object from the database";
            s.Responses[200] = $"{typeof(TEntity).Name} object and OK response if GET operation was successful";
            s.Responses[404] =
                "Not Found response if GET operation was performed for on an Entity that could not be found in the database";
            s.ResponseExamples[404] = new {};
            s.Params["id"] = $"id derived from the {typeof(TEntity).Name} object";
        });
    }

    public override async Task HandleAsync(Entity req, CancellationToken ct)
    {
        var retrieved = this._repository.Retrieve(req.Id);

        if (retrieved is not null)
        {
            await this.SendResultAsync(TypedResults.Ok(retrieved));
            return;
        }

        await this.SendResultAsync(TypedResults.NotFound());
    }
}
