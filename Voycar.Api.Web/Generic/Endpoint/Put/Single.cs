namespace Voycar.Api.Web.Generic.Endpoint.Put;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<TEntity, Results<Ok, NotFound>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly string[] Roles;

    protected Single(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.Roles = roles;
    }

    public override void Configure()
    {
        this.Put(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.Roles);
        this.Summary(s =>
        {
            s.Summary = $"Update {typeof(TEntity).Name}";
            s.Description = $"Update {typeof(TEntity).Name} object in the database";
            s.Responses[200] = "If PUT operation is successful";
            s.Responses[404] = "If PUT operation is performed for an Entity that could not be found in the database";
            s.Params["id"] = $"ID of the {typeof(TEntity).Name} object to update";
        });
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var updated = this._repository.Update(req);

        if (updated)
        {
            await this.SendResultAsync(TypedResults.Ok());
            return;
        }

        await this.SendResultAsync(TypedResults.NotFound());
    }
}
