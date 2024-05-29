namespace Voycar.Api.Web.Generic.Endpoint;

public class Authorization : Group
{
    public Authorization()
    {
        // configuration to use for every endpoint in this group
        this.Configure("", endpoint =>
        {
            endpoint.AllowAnonymous();
            // should go here at some point
            //endpoint.Roles(endpoint_role); // e.g. "admin"
        });
    }
}
