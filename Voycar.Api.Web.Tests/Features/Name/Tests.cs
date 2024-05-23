namespace Voycar.Api.Web.Tests.Features.Name;

using Voycar.Api.Web.Features.Name.GetHello;

public class Tests(App app) : TestBase<App>
{
    [Fact]
    public async Task ReturnsExpectedType()
    {
        var (rsp, res) = await app.Client.POSTAsync<Endpoint, Request, Response>(new Request
        {
            FirstName = "Foo",
            LastName = "Bar"
        });

        res.Should().BeOfType<Response>();
    }

    [Fact]
    public async Task ReturnsExpectedStatusCode()
    {
        var (rsp, res) = await app.Client.POSTAsync<Endpoint, Request, Response>(new Request
        {
            FirstName = "Foo",
            LastName = "Bar"
        });

        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
