using LoggingMicroservice.Interfaces;
using LoggingMicroservice.RabbitMQ;
using MyAiropsLogging.CentralizedLogging.Interfaces;
using MyAiropsLogging.CentralizedLogging.Services;
using RabbitMQ.Client;
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
builder.Services.AddSingleton<ILoggingService, LoggingService>();

builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnection>();

var app = builder.Build();
Log.Logger.Information($"Microservice MyAiropsLogging.CentralizedLogging started");
