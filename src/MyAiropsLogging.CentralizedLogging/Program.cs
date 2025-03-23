using Serilog;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .MinimumLevel.Information()
            .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

var app = builder.Build();
Log.Logger.Information($"Microservice MyAiropsLogging.CentralizedLogging started");
