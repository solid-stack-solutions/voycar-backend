namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<Entity, Results<Ok<TEntity>, NotFound>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> repository;
    protected new readonly string[] Roles;

    protected Single(IRepository<TEntity> repository, string[] roles)
    {
        this.repository = repository;
        this.Roles = roles;
    }

    public override void Configure()
    {
        this.Get(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.Roles);
        this.Summary(s =>
        {
            s.Summary = $"Retrieve {typeof(TEntity).Name}";
            s.Description = $"Retrieve {typeof(TEntity).Name} object from the database";
            s.Responses[200] = $"{typeof(TEntity).Name} object if GET operation is successful";
            s.Responses[404] = "If GET operation is performed for an entity that could not be found in the database";
            s.Params["id"] = $"ID of the {typeof(TEntity).Name} object to retrieve";
        });
    }

    public override async Task HandleAsync(Entity req, CancellationToken ct)
    {
        var retrieved = this.repository.Retrieve(req.Id);

        if (retrieved is not null)
        {
            await this.SendResultAsync(TypedResults.Ok(retrieved));
            return;
        }

        await this.SendResultAsync(TypedResults.NotFound());
    }
}
