namespace Voycar.Api.Web.Tests.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Context;
using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

[DisableWafCache]
public class App : AppFixture<Program>
{
    // Test with prod Db
    private const string ConnectionString = "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb;";
    // Test in a test-container
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16.3")
        .WithDatabase("VoycarDb-Tests")
        .WithUsername("admin")
        .WithPassword("admin")
        .Build();

    private VoycarDbContext _context;

    protected internal VoycarDbContext GetContext() => this._context;

    // See: https://gist.github.com/dj-nitehawk/04a78cea10f2239eb81c958c52ec84e0
    protected override Task PreSetupAsync()
    {
        return this._container.StartAsync();
    }

    protected override Task SetupAsync()
    {
        // Place one-time setup code here
        this._context = this.Services.GetRequiredService<VoycarDbContext>();

        this._context.Database.EnsureDeleted();
        this._context.Database.EnsureCreated();
        return Task.CompletedTask;
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        // Do host builder config here
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Do test service registration here
        var descriptor =
            services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<VoycarDbContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<VoycarDbContext>(options =>
        {
            // HINT: Change ConnectionString here to test against prod-database
            // WARNING: This WILL delete your existing data
            options.UseNpgsql(this._container.GetConnectionString());
        });
    }

    protected override Task TearDownAsync()
    {
        // Do cleanups here
        return this._container.StopAsync();
    }
}
