using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using MassTransit;
using LoggingMicroservice.Interfaces;
using LoggingMicroservice.RabbitMQ;
using MyAiropsLogging.Shared;
using MyAiropsLogging.CentralizedLogging.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();


builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnection>();
builder.Services.AddSingleton(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));

var app = builder.Build();

var rabbitMQPublisher = app.Services.GetRequiredService<IRabbitMQPublisher<LogMessageDto>>();
var logMessage = new LogMessageDto { MessageTemplate = "Microservice Main Started", Level = LevelDto.Information };

// Send a log message
await rabbitMQPublisher.PublishMessageAsync(logMessage, RabbitQueues.LogsQ);

app.MapGet("/", () => "Microservice Main on");
app.MapControllers();
app.Run();
