namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Repository;

public abstract class All<TEntity>
    : EndpointWithoutRequest<IEnumerable<TEntity>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> _repository;
    protected new readonly string[] Roles;

    protected All(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.Roles = roles;
    }

    public override void Configure()
    {
        this.Get(typeof(TEntity).Name.ToLowerInvariant() + "/all");

        if (this.Roles.Length > 0)
        {
            this.Roles(this.Roles);
        }
        else
        {
            this.AllowAnonymous();
        }

        this.Summary(s =>
        {
            s.Summary = $"Retrieve all {typeof(TEntity).Name}s";
            s.Description = $"Retrieve all {typeof(TEntity).Name} objects from the database";
            s.Responses[200] = $"All {typeof(TEntity).Name} objects (may be an empty array)";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await this.SendResultAsync(TypedResults.Ok(this._repository.RetrieveAll()));
    }
}
