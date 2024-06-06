namespace Voycar.Api.Web.Generic.Endpoint.Post;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<TEntity, Results<Ok, NoContent>>
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
        this.Post(typeof(TEntity).Name.ToLowerInvariant());
        this.Roles(this.roles);
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var created = this._repository.Create(req);

        if (created)
        {
            await this.SendResultAsync(TypedResults.Ok());
            return;
        }
        
        await this.SendResultAsync(TypedResults.NoContent());
    }
}
