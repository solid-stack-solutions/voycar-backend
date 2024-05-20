var builder = WebApplication.CreateBuilder(args);

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
app.UseSwaggerGen();

app.Run();

public partial class Program {}
