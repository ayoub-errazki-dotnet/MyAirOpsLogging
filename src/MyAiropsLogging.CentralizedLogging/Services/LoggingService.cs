using MyAiropsLogging.Shared;
using MyAiropsLogging.CentralizedLogging.RabbitMQ;
using MyAiropsLogging.CentralizedLogging.Interfaces;

using Serilog;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MyAiropsLogging.CentralizedLogging.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly Serilog.ILogger _logger;
        private readonly IRabbitMQConnectionFactory _rabbitMQConnectionFactory;
        public LoggingService(Serilog.ILogger logger, IRabbitMQConnectionFactory rabbitMQConnectionFactory)
        {
            _logger = logger;
            _rabbitMQConnectionFactory = rabbitMQConnectionFactory;
        }

        public string TruncateLog(string message)
        {
            if (!string.IsNullOrEmpty(message) && message.Length > 255)
            {
                message = message[..252] + "...";
            }
            return message;
        }

        public async Task ListenForIncomingLogs()
        {
            var connectionFactory = _rabbitMQConnectionFactory.Connection()
                ?? throw new InvalidOperationException("RabbitMQ connection factory is not initialized.");

            using var connection = await connectionFactory.CreateConnectionAsync();
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

                if (logMessage != null)
                {
                    logMessage.CorrelationID = ea.BasicProperties.CorrelationId;
                    LogMessage(logMessage);
                }
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(RabbitQueues.LogsQ, autoAck: true, consumer: consumers);
            Console.ReadLine();
        }

        public void LogMessage(LogMessageDto log)
        {
            if (log == null || string.IsNullOrEmpty(log.MessageTemplate))
                throw new ArgumentException("Log Message is required");

            log.MessageTemplate = TruncateLog(log.MessageTemplate);

            var idFormatted = !string.IsNullOrEmpty(log.CorrelationID) ? $" With CorrelationID [{log.CorrelationID}]" : string.Empty;
            var logFormatted = $"  [{log.MessageTemplate}] {idFormatted}";

            switch (log.Level)
            {
                case LevelDto.Error:
                    _logger.Error(logFormatted);
                    break;
                case LevelDto.Warning:
                    _logger.Warning(logFormatted);
                    break;
                case LevelDto.Fatal:
                    _logger.Fatal(logFormatted);
                    break;
                default:
                    _logger.Information(logFormatted);
                    break;
            }
        }
    }
}
