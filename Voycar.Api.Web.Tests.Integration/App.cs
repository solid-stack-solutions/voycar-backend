namespace Voycar.Api.Web.Tests.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Context;
using Setup;
using Testcontainers.PostgreSql;

public class App : AppFixture<Program>
{
    /* Change this constant to true, to test against production database
    WARNING: This will DELETE your existing data */
    private const bool TestAgainstProductionDb = false;
    private PostgreSqlContainer Container { get; set; }
    private string ConnectionString { get; set; }
    public VoycarDbContext Context { get; private set; }
    public HttpClient Member { get; private set; }
    public HttpClient Employee { get; private set; }
    public HttpClient Admin { get; private set; }

    // See: https://gist.github.com/dj-nitehawk/04a78cea10f2239eb81c958c52ec84e0
    protected override async Task PreSetupAsync()
    {
        if (TestAgainstProductionDb)
        {
            // WARNING: This will test against production database and DELETE existing data
            this.ConnectionString =
                "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb;";
            return;
        }

        // Create a Db test container to run tests against
        this.Container = new PostgreSqlBuilder()
            .WithImage("postgres:16.3")
            .WithDatabase("pgsql-testcontainer")
            .WithUsername("admin")
            .WithPassword("admin")
            .WithExposedPort(5241)
            .Build();
        await this.Container.StartAsync();
        this.ConnectionString = this.Container.GetConnectionString();
    }

    protected override async Task SetupAsync()
    {
        // == Place one-time setup code here ==

        // Set up Db for tests
        this.Context = this.Services.GetRequiredService<VoycarDbContext>();
        await this.Context.Database.EnsureDeletedAsync(); // Delete data on Db
        await this.Context.Database.EnsureCreatedAsync(); // Populate with Db schema

        // Create custom HttpClients to use in tests
        this.Member = await ClientFactory.CreateMemberClient(this);
        this.Employee = await ClientFactory.CreateEmployeeClient(this, this.Context);
        this.Admin = await ClientFactory.CreateAdminClient(this, this.Context);
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        // == Do host builder config here ==
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // == Do test service registration here ==
        var descriptor = services.SingleOrDefault(service =>
            service.ServiceType == typeof(DbContextOptions<VoycarDbContext>)
        );
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<VoycarDbContext>(options =>
        {
            options.UseNpgsql(this.ConnectionString);
        });
    }

    protected override Task TearDownAsync()
    {
        // == Do cleanups here ==

        this.Member.Dispose();
        this.Employee.Dispose();
        this.Admin.Dispose();
        return this.Container.StopAsync();
    }
}
