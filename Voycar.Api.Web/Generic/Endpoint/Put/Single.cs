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
        Description(b => b
                .Accepts<TEntity>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(404),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = $"PUT Endpoint for {typeof(TEntity).Name}";
            s.Description = $"This Endpoint ist used to update {typeof(TEntity).Name} Objects in the database";
            s.Responses[200] = "OK response if PUT operation was successful";
            s.Responses[404] =
                "Not Found response if PUT operation was performed for on an Entity that could not be found in the database";
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
