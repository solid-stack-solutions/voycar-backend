namespace Voycar.Api.Web.Generic.Endpoint.Get;

using Repository;

public abstract class All<TEntity>
    : EndpointWithoutRequest<IEnumerable<TEntity>>
    where TEntity : Entity
{
    protected readonly IRepository<TEntity> _repository;
    protected readonly string[] roles;

    protected All(IRepository<TEntity> repository, string[] roles)
    {
        this._repository = repository;
        this.roles = roles;
    }

    public override void Configure()
    {
        this.Get(typeof(TEntity).Name.ToLowerInvariant() + "/all");
        this.Roles(this.roles);
        this.Summary(s =>
        {
            s.Summary = $"Retrieve all {typeof(TEntity).Name} objects";
            s.Description = $"Retrieve all {typeof(TEntity).Name} objects from the database";
            s.Responses[200] = $"All {typeof(TEntity).Name} objects";
            s.Responses[404] = "If requesting user isn't authorized";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await this.SendResultAsync(TypedResults.Ok(this._repository.RetrieveAll()));
    }
}
