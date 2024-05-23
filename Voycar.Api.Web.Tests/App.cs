namespace Voycar.Api.Web.Tests;

using Microsoft.AspNetCore.Hosting;
using Web.Features.Name.GetHello;

public class App : AppFixture<Program>
{
    protected override Task SetupAsync()
    {
        // place one-time setup code here
        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        // do host builder config here
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        // do test service registration here
    }

    protected override Task TearDownAsync()
    {
        // do cleanups here
        return Task.CompletedTask;
    }
}
