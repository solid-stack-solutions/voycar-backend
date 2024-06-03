namespace Voycar.Api.Web.Tests.Features.Roles;

using Entities;

public class Tests : TestBase<App>
{
    private readonly App app;

    public Tests(App app)
    {
        this.app = app;
    }

    [Fact]
    public async Task ExamplePost()
    {
        var httpResponseMessage =
            await this.app.Client.POSTAsync<Web.Features.Roles.Endpoints.Post.Single, Role>(new Role()
            {
                Name = "admin"
            });

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
