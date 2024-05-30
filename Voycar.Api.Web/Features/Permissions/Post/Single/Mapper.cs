namespace Voycar.Api.Web.Features.Permissions.Post.Single;

using Entities;

public class Mapper : RequestMapper<Request, Permission>
{
    public override Permission ToEntity(Request r)
    {
        return new Permission() { Name = r.Name };
    }
}
