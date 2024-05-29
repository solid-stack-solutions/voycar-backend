namespace Voycar.Api.Web.Features.Permissions.Post.Single;

using Microsoft.AspNetCore.Http.HttpResults;
using Repository;

public class Endpoint : Endpoint<Request, Results<Ok, NoContent>, Mapper>
{
    private readonly IPermissions _permissions;

    public Endpoint(IPermissions permissions)
    {
        this._permissions = permissions;
    }

    public override void Configure()
    {
        this.Post("/permission/single");
        this.AllowAnonymous();
        // should go here at some point
        //this.Roles("Admin");
        //this.Permissions("Permission");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var created = this._permissions.CreatePermission(this.Map.ToEntity(req));
        await this.SendResultAsync(created ? TypedResults.Ok() : TypedResults.NoContent());
    }
}
