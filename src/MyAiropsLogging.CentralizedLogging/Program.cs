using DotNetEnv;
using LoggingMicroservice.Interfaces;
using LoggingMicroservice.RabbitMQ;
using MyAiropsLogging.CentralizedLogging.Interfaces;
using MyAiropsLogging.CentralizedLogging.Services;
using RabbitMQ.Client;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using MassTransit.Logging;
using MyAiropsLogging.Shared;

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
Log.Logger.Information($"Microservice MyAiropsLogging.CentralizedLogging started");

var loggingService = app.Services.GetRequiredService<ILoggingService>();

#region RabbitMQ consuming listener
var factory = app.Services.GetRequiredService<IConnectionFactory>();
var rabbitConnection = app.Services.GetRequiredService<IRabbitMQConnectionFactory>();

using var connection = await rabbitConnection.Connection().CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();


await channel.QueueDeclareAsync(queue: RabbitQueues.LogsQ, durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine(" [*] Waiting for incoming logs...");

var consumers = new AsyncEventingBasicConsumer(channel);
consumers.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"** Received Log {message}");
    var logMessage = JsonSerializer.Deserialize<LogMessageDto>(message);

    if(logMessage != null)
    loggingService.LogMessage(logMessage);
    
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(RabbitQueues.LogsQ, autoAck: true, consumer: consumers);
Console.ReadLine();
#endregion RabbitMQ consuming listener
