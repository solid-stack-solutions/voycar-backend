namespace Voycar.Api.Web.Generic.Endpoint.Put;

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
        this.Description(b => b
                .Accepts<TEntity>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(404),
            clearDefaults: true);
        this.Summary(s =>
        {
            s.Summary = $"Update {typeof(TEntity).Name}";
            s.Description = $"Endpoint to update {typeof(TEntity).Name} Objects in the database";
            s.Responses[200] = "If PUT operation is successful";
            s.Responses[404] =
                "If PUT operation is performed for an Entity that could not be found in the database or requesting user isn't authorized";
            s.ResponseExamples[404] = new {};
        });
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
