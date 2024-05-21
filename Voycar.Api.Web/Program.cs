Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting application!");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    builder.Services.AddFastEndpoints();
    builder.Services.SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Voycar Web API Documentation";
            s.Version = "v1";
        };
    });

    var app = builder.Build();

    app.UseFastEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerGen();
    }

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program {}
