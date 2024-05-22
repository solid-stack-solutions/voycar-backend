namespace Voycar.Api.Web.Tests;

using Microsoft.AspNetCore.Hosting;

public class App : AppFixture<Program>
{
    protected override Task SetupAsync()
    {
        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {

    }

    protected override void ConfigureServices(IServiceCollection s)
    {

    }

    protected override Task TearDownAsync()
    {
        return Task.CompletedTask;
    }
}
