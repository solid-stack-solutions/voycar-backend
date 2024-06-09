namespace Voycar.Api.Web.Generic.Endpoint.Delete;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<Entity, Results<Ok, NotFound>>
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
        this.Delete(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.roles);
        this.Summary(s =>
        {
            s.Summary = $"Delete {typeof(TEntity).Name}";
            s.Description = $"Remove {typeof(TEntity).Name} objects from the database";
            s.Responses[200] = "If DELETE operation is successful";
            s.Responses[404] =
                "If DELETE operation is performed for an Entity that could not be found in the database or requesting user isn't authorized";
            s.Params["id"] = $"ID derived from the {typeof(TEntity).Name} object";
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
