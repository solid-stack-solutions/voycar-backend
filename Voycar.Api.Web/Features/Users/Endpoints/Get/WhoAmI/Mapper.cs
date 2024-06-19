namespace Voycar.Api.Web.Features.Users.Endpoints.Get.WhoAmI;

using Entities;

public class Mapper : ResponseMapper<Response, User>
{
    public override Response FromEntity(User e)
    {
        return new Response() { Email = e.Email };
    }
}
