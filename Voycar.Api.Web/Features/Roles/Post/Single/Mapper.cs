namespace Voycar.Api.Web.Features.Roles.Post.Single;

using Entities;

public class Mapper : RequestMapper<Request, Role>
{
    public override Task<Role> ToEntityAsync(Request r, CancellationToken ct = new CancellationToken())
    {
        return Task.FromResult(new Role() { Name = r.Name });
    }
}
