var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

builder.Services.AddControllers();

app.MapGet("/", () => "Service Alpha on");

app.MapControllers();

app.Run();
