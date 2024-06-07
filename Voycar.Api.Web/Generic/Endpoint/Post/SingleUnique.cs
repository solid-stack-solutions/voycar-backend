namespace Voycar.Api.Web.Generic.Endpoint.Post;

using Repository;

/// <summary>
///     Uses <see cref="IRepository{T}.CreateUnique"/> (instead of <see cref="IRepository{T}.Create"/>)
/// </summary>
public abstract class SingleUnique<TEntity>
    : Single<TEntity>
    where TEntity : Entity
{
    protected SingleUnique(IRepository<TEntity> repository, string[] roles)
        : base(repository, roles) {}

    public override void Configure()
    {
        this.Post(typeof(TEntity).Name.ToLowerInvariant());
        this.Roles(this.roles);
        // TODO Swagger documentation
        //this.Description();
        //this.Summary();
        this.Description(b => b
                .Accepts<TEntity>("Voycar.Api.Web/Generic/Entity")
                .Produces<IResult>(200)
                .ProducesProblem(204)
                .ProducesProblem(404),
            clearDefaults: true);
        this.Summary(s =>
        {
            s.Summary = $"Create unique {typeof(TEntity).Name}";
            s.Description = $"Endpoint to add new unique {typeof(TEntity).Name} objects into the database";
            s.Responses[200] = "If POST operation is successful";
            s.Responses[204] =
                $"If POST operation failed or the same {typeof(TEntity).Name} object is already present in the database";
            s.Responses[404] = "If requesting user isn't authorized";
        });
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var created = this._repository.CreateUnique(req);

        if (created)
        {
            await this.SendResultAsync(TypedResults.Ok());
            return;
        }

        await this.SendResultAsync(TypedResults.NoContent());
    }
}
