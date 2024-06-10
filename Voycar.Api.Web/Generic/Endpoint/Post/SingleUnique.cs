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
        this.Summary(s =>
        {
            s.Summary = $"Create unique {typeof(TEntity).Name}";
            s.Description = $"Add new unique {typeof(TEntity).Name} object into the database";
            s.Responses[200] = "Generated ID if POST operation is successful";
            s.Responses[204] =
                $"If POST operation failed or the same {typeof(TEntity).Name} object is already present in the database";
            s.Responses[404] = "If requesting user isn't authorized";
            s.ResponseExamples = new Dictionary<int, object> {{ 200, new Entity { Id = Guid.NewGuid() }}};
        });
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var guid = this._repository.CreateUnique(req);

        if (guid is not null)
        {
            await this.SendResultAsync(TypedResults.Ok(new Entity { Id = guid.Value }));
            return;
        }

        await this.SendResultAsync(TypedResults.NoContent());
    }
}
