namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<Entity, Results<Ok<TEntity>, NotFound>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly string[] roles;

    protected Single(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.roles = roles;
    }

    public override void Configure()
    {
        this.Get(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.roles);
        this.Summary(s =>
        {
            s.Summary = $"Retrieve {typeof(TEntity).Name}";
            s.Description =
                $"Retrieve a {typeof(TEntity).Name} object from the database";
            s.Responses[200] = $"{typeof(TEntity).Name} object if GET operation is successful";
            s.Responses[404] =
                "If GET operation is performed for an Entity that could not be found in the database or requesting user isn't authorized";
            s.Params["id"] = $"ID derived from the {typeof(TEntity).Name} object";
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
