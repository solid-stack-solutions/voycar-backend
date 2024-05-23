namespace Voycar.Api.Web.Tests.Features.Name;

using Voycar.Api.Web.Features.Name.GetHello;

public class Tests(App app) : TestBase<App>
{
    [Fact]
    public async Task ReturnsExpectedType()
        // [3]
    {
        var (rsp, res) = await app.Client.POSTAsync<Endpoint, Request, Response>(new Request
        {
            FirstName = "Foo",
            LastName = "Bar"
        });

        res.Should().BeOfType<Response>();
        rsp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
