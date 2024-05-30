namespace Voycar.Api.Web.Generic.Endpoint.Post;

using Microsoft.AspNetCore.Http.HttpResults;
using Repository;

public abstract class Single<TEntity, TRequest, TMapper>
    : Endpoint<TRequest, Results<Ok, NoContent>, TMapper>
    where TEntity : Entity
    where TRequest : class
    where TMapper : RequestMapper<TRequest, TEntity>
{
    private readonly IRepository<TEntity> _repository;
    private readonly string route;
    private readonly string[] roles;

    protected Single(IRepository<TEntity> repository, string route, string[] roles)
    {
        this._repository = repository;
        this.route = route;
        this.roles = roles;
    }

    public override void Configure()
    {
        this.Post(this.route);
        this.Roles(this.roles);
    }

    public override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        var created = this._repository.Create(await this.Map.ToEntityAsync(req, ct));
        await this.SendResultAsync(created ? TypedResults.Ok() : TypedResults.NoContent());
    }
}
