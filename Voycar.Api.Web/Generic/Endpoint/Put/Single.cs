namespace Voycar.Api.Web.Generic.Endpoint.Put;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<TEntity, Results<Ok, NotFound>>
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
        this.Put(typeof(TEntity).Name.ToLowerInvariant());
        this.Roles(this.roles);
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
