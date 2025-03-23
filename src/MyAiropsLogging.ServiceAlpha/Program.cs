using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using MassTransit;
using LoggingMicroservice.Interfaces;
using LoggingMicroservice.RabbitMQ;
using MyAiropsLogging.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();


builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
builder.Services.AddSingleton<IRabbitMQConnectionFactory, RabbitMQConnection>();
builder.Services.AddScoped(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
builder.Services.AddSingleton(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));


var app = builder.Build();

var rabbitConnection = app.Services.GetRequiredService<IRabbitMQConnectionFactory>();
var factory = app.Services.GetRequiredService<IConnectionFactory>();
using var connection = await rabbitConnection.Connection().CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
var rabbitMQPublisher = app.Services.GetRequiredService<IRabbitMQPublisher<LogMessageDto>>();

var logMessage = new LogMessageDto { MessageTemplate = "Microservice Main Started", Level = LevelDto.Information };
await rabbitMQPublisher.PublishMessageAsync(logMessage, RabbitQueues.LogsQ);

app.MapGet("/", () =>  "Microservice Main on");

app.MapControllers();

app.Run();
