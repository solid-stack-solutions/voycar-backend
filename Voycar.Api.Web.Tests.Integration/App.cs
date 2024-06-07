﻿namespace Voycar.Api.Web.Tests.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Context;
using Testcontainers.PostgreSql;

using Login = Features.Members.Post.Login;
using Registration = Features.Members.Post.Registration;
using Verify = Features.Members.Get.Verify;

public class App : AppFixture<Program>
{
    // Use this to test with prod Db, see ConfigureServices()
    private const string ConnectionString =
        "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb;";

    // Create a Db test container to run tests against
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16.3")
        .WithDatabase("pgsql-testcontainer")
        .WithUsername("admin")
        .WithPassword("admin")
        .WithExposedPort(5000)
        .Build();

    public VoycarDbContext Context { get; private set; }
    private HttpClient Admin { get; set; }

    // See: https://gist.github.com/dj-nitehawk/04a78cea10f2239eb81c958c52ec84e0
    protected override Task PreSetupAsync()
    {
        return this._container.StartAsync();
    }

    protected override async Task SetupAsync()
    {
        // Place one-time setup code here

        // Db setup
        this.Context = this.Services.GetRequiredService<VoycarDbContext>();
        await this.Context.Database.EnsureDeletedAsync(); // Delete data on Db
        await this.Context.Database.EnsureCreatedAsync(); // Populate with Db schema

        // Populate Db
        const string testMail = "integration@test.de";
        const string password = "integration";

        var (regHttpRsp, regBodyRes) =
            await this.Client.POSTAsync<Registration.Endpoint, Registration.Request, Registration.Response>(
                new Registration.Request
                {
                    Email = testMail,
                    Password = password,
                    FirstName = "...",
                    LastName = "...",
                    Street = "...",
                    HouseNumber = "...",
                    PostalCode = "...",
                    City = "...",
                    BirthDate = new DateOnly(2000, 1, 1),
                    BirthPlace = "...",
                    PhoneNumber = "...",
                    DriversLicenseNumber = "...",
                    IdCardNumber = "..."
                }
            );

        var verHttpRsp = await this.Client.GETAsync<Verify.Endpoint, Verify.Request>(new Verify.Request
        {
            VerificationToken = regBodyRes.VerificationToken
        });

        await this.Client.POSTAsync<Login.Endpoint, Login.Request>(new Login.Request
        {
            Email = testMail, Password = password
        });
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
