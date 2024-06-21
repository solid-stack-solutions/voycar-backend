namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Repository;

public abstract class All<TEntity>
    : EndpointWithoutRequest<IEnumerable<TEntity>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> repository;
    protected new readonly string[] Roles;

    protected All(IRepository<TEntity> repository, string[] roles)
    {
        this.repository = repository;
        this.Roles = roles;
    }

    public override void Configure()
    {
        this.Get(typeof(TEntity).Name.ToLowerInvariant() + "/all");
        this.Roles(this.Roles);
        this.Summary(s =>
        {
            s.Summary = $"Retrieve all {typeof(TEntity).Name}s";
            s.Description = $"Retrieve all {typeof(TEntity).Name} objects from the database";
            s.Responses[200] = $"All {typeof(TEntity).Name} objects (may be an empty array)";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await this.SendResultAsync(TypedResults.Ok(this.repository.RetrieveAll()));
    }
}
