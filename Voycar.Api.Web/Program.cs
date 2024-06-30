using Microsoft.AspNetCore.Authentication.Cookies;
using Voycar.Api.Web.Context;
using Voycar.Api.Web.Features.Cars.Repository;
using Voycar.Api.Web.Features.Cities.Repository;
using Voycar.Api.Web.Features.Roles.Repository;
using Voycar.Api.Web.Features.Members.Repository;
using Voycar.Api.Web.Features.Plans.Repository;
using Voycar.Api.Web.Features.Reservations.Repository;
using Voycar.Api.Web.Features.Stations.Repository;
using Voycar.Api.Web.Features.Users.Repository;
using Voycar.Api.Web.Service;

try
{
    // Load .env file from current directory
    // If there is none: search parent directories
    DotNetEnv.Env.TraversePath().Load();

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
    builder.Services.AddTransient<ICars, Cars>();
    builder.Services.AddTransient<ICities, Cities>();
    builder.Services.AddTransient<IPlans, Plans>();
    builder.Services.AddTransient<IStations, Stations>();
    builder.Services.AddTransient<IReservations, Reservations>();

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
        options.EnableJWTBearerAuth = false;
    });
    builder.Services.AddCors(options =>
    {
        // Allow CORS requests from frontend with cookie. Origins and Headers have to be explicitly set and
        // can not use any wildcards.
        // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS#requests_with_credentials
        //      "Credentialed requests and wildcards"
        options.AddDefaultPolicy(
            policy =>
            {
                policy.WithOrigins("http://localhost:5173"); // Frontend URL, change this if frontend URL changes
                policy.WithHeaders("content-type");
                policy.AllowCredentials(); // Necessary to allow cookie authentication
                policy.WithMethods(["POST", "GET", "DELETE", "PUT"]);
            });
    });

    var app = builder.Build();


    // Caution: Swagger available in production environment
    app.UseSerilogRequestLogging()
       .UseRouting()
       .UseCors()
       .UseAuthentication()
       .UseAuthorization()
       .UseFastEndpoints(c =>
       {
           c.Endpoints.Configurator = ep =>
           {
               // Setup Swagger Documentation for all endpoints which require a role for accessing
               if (ep.AllowedRoles is null || ep.AllowedRoles.Count == 0)
               {
                   return;
               }

               ep.Description(b =>
               {
                   b.Produces(401); // Unauthorized -> if user is not logged in
                   b.Produces(403); // Forbidden -> if user is missing roles
               });
               ep.Summary(s =>
               {
                   var existingDescription = s.Description;
                   var rolesDescription = $"\n\nRequired role: {(ep.AllowedRoles.Count > 0 ? string.Join(", ", ep.AllowedRoles) : "")}";
                   s.Description = existingDescription + rolesDescription;
                   s.Responses[401] = "If requesting user isn't authorized";
                   s.Responses[403] = "If requesting user doesn't have the required roles";
               });
           };
       })
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
