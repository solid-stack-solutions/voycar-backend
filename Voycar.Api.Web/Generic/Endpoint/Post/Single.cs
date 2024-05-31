namespace Voycar.Api.Web.Generic.Endpoint.Post;

using Microsoft.AspNetCore.Http.HttpResults;
using Repository;

public abstract class Single<TEntity>
    : Endpoint<TEntity, Results<Ok, NoContent>>
    where TEntity : Entity
{
    private readonly IRepository<TEntity> _repository;
    private readonly string[] roles;

    protected Single(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.roles = roles;
    }

    public override void Configure()
    {
        this.Post(typeof(TEntity).Name);
        this.Roles(this.roles);
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var created = this._repository.Create(req);

        if (created)
        {
            await this.SendResultAsync(TypedResults.Ok());
        }

        await this.SendResultAsync(TypedResults.NoContent());
    }
}