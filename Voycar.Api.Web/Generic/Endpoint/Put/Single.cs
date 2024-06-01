namespace Voycar.Api.Web.Generic.Endpoint.Put;

using System.Globalization;
using Repository;

public abstract class Single<TEntity>
    : Endpoint<TEntity, Results<Ok, NotFound>>
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
        this.Put(typeof(TEntity).Name.ToLowerInvariant());
        this.Roles(this.roles);
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var updated = this._repository.Update(req);

        if (updated)
        {
            await this.SendResultAsync(TypedResults.Ok());
        }

        await this.SendResultAsync(TypedResults.NotFound());
    }
}
