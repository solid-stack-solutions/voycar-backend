namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Microsoft.AspNetCore.Http.HttpResults;
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
    }

    public override async Task HandleAsync(Entity req, CancellationToken ct)
    {
        var retrieved = this._repository.Retrieve(req.Id);

        if (retrieved is null)
        {
            await this.SendResultAsync(TypedResults.NotFound());
        }

        await this.SendResultAsync(TypedResults.Ok(retrieved));
    }
}
