var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => Results.Redirect("/health"));

app.MapReverseProxy();

app.MapGet("/health", () => Results.Ok(new { service = "HotelGateway", status = "ok", utc = DateTime.UtcNow }));

app.Run();
