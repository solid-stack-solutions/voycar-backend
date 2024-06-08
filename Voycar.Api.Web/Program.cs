using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Voycar.Api.Web.Context;
using Voycar.Api.Web.Features.Roles.Repository;
using Voycar.Api.Web.Features.Members.Repository;
using Voycar.Api.Web.Features.Members.Services.EmailService;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddDbContext<VoycarDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("VoycarDb"));
    });


    builder.Services
        .AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(10), options =>
        {
            /* Instruct the handler to re-issue a new cookie with a new expiration time any
                time it processes a request which is more than halfway through the expiration window */
            options.SlidingExpiration = true;

            /* Override ASP.NET default cookie middleware, since that tries to redirect to /Account/Login/
                after an unauthorized request */
            options.Events = new CookieAuthenticationEvents()
            {
                OnRedirectToLogin = (ctx) =>
                {
                    if (ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = 401;
                    }
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = (ctx) =>
                {
                    if (ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = 403;
                    }
                    return Task.CompletedTask;
                }
            };
        } )
        .AddAuthorization();

    // Repositories
    builder.Services.AddTransient<IRoles, Roles>();
    builder.Services.AddTransient<IMembers, Members>();
    builder.Services.AddTransient<IUsers, Users>();

    // Services
    builder.Services.AddTransient<IEmailService, EmailService>();

    builder.Services.AddFastEndpoints();
    builder.Services.SwaggerDocument(options =>
    {
        options.DocumentSettings = settings =>
        {
            settings.Title = "Voycar Web API Documentation";
            settings.Version = "v1";
        };
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Caution: Swagger available in production environment
    app.UseSerilogRequestLogging()
       .UseAuthentication()
       .UseAuthorization()
       .UseFastEndpoints()
       .UseSwaggerGen();

    app.Run();
}
// Ignore unnecessary log message when creating migrations
// https://stackoverflow.com/questions/70247187/microsoft-extensions-hosting-hostfactoryresolverhostinglistenerstopthehostexce
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program {}
