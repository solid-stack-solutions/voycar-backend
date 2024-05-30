namespace Voycar.Api.Web.Features.Permissions.Post.Single;

using Entities;

public class Mapper : RequestMapper<Request, Role>
{
    public override Role ToEntity(Request r)
    {
        return new Role() { Name = r.Name };
    }
}
