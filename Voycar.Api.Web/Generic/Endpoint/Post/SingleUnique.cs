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
