namespace Voycar.Api.Web.Generic.Endpoint.Post;

using Repository;

public abstract class Single<TEntity>
    : Endpoint<TEntity, Results<Ok<Entity>, NoContent>>
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
        this.Summary(s =>
        {
            s.Summary = $"Create {typeof(TEntity).Name}";
            s.Description = $"Add new {typeof(TEntity).Name} object into the database";
            s.Responses[200] = "Generated ID if POST operation is successful";
            s.Responses[204] =
                "If POST operation failed";
            s.Responses[404] = "If requesting user isn't authorized";
            s.ResponseExamples = new Dictionary<int, object> {{ 200, new Entity { Id = new Guid() }}};
        });
    }

    public override async Task HandleAsync(TEntity req, CancellationToken ct)
    {
        var guid = this._repository.Create(req);

        if (guid is not null)
        {
            await this.SendResultAsync(TypedResults.Ok(new Entity { Id = guid.Value }));
            return;
        }

        await this.SendResultAsync(TypedResults.NoContent());
    }
}
