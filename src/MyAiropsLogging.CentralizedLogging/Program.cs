using DotNetEnv;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MassTransit.Logging;

using Serilog;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

using MyAiropsLogging.CentralizedLogging.Interfaces;
using MyAiropsLogging.CentralizedLogging.Services;
using MyAiropsLogging.CentralizedLogging.RabbitMQ;

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
builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnection>();

var app = builder.Build();
var loggingService = app.Services.GetRequiredService<ILoggingService>();
Log.Logger.Information($"Microservice MyAiropsLogging.CentralizedLogging started");

// Listen for incoming logs
await loggingService.ListenForIncomingLogs();