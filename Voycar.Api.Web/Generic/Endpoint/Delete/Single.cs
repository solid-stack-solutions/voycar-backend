namespace Voycar.Api.Web.Generic.Endpoint.Delete;

using Repository;

public class Single<TEntity>
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
        this.Delete(typeof(TEntity).Name + "/{id}");
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
