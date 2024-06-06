namespace Voycar.Api.Web.Generic.Endpoint.Post;

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
        this.Post(typeof(TEntity).Name.ToLowerInvariant());
        this.Roles(this.roles);
        Description(b => b
                .Accepts<TEntity>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(404),
            clearDefaults: true);
        Summary(s =>
        {
            s.Summary = $"POST Endpoint for {typeof(TEntity).Name}";
            s.Description = $"This Endpoint is used to add new {typeof(TEntity).Name} objects into the database";
            s.Responses[200] = "OK response if POST operation was successful";
            s.Responses[404] =
                "Not Found response if POST operation was performed for on an Entity that could not be found in the database";
            s.ResponseExamples[404] = new {};
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
