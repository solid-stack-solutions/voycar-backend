namespace Voycar.Api.Web.Generic.Endpoint.Delete;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<Entity, Results<Ok, NotFound>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly string[] roles;

    public Single(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.roles = roles;
    }

    public override void Configure()
    {
        this.Delete(typeof(TEntity).Name.ToLowerInvariant() + "/{id}");
        this.Roles(this.roles);
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
