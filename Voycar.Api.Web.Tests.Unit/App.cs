using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Voycar.Api.Web.Tests.Unit;

public class App : AppFixture<Program>
{
    protected override Task SetupAsync()
    {
        // Place one-time setup code here
        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        // Do host builder config here
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        // Do test service registration here
    }

    protected override Task TearDownAsync()
    {
        // Do cleanups here
        return Task.CompletedTask;
    }
}
