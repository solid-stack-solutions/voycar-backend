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
        this.Description(b => b
                .Accepts<TEntity>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(204)
                .ProducesProblem(404),
            clearDefaults: true);
        this.Summary(s =>
        {
            s.Summary = $"Create {typeof(TEntity).Name}";
            s.Description = $"Endpoint to add new {typeof(TEntity).Name} objects into the database";
            s.Responses[200] = "If POST operation is successful";
            s.Responses[204] =
                "If POST operation is performed for on an Entity that could not be found in the database";
            s.Responses[404] = "If requesting user isn't authorized";
        });
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
