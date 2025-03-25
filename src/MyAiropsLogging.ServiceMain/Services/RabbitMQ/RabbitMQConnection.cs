using MyAiropsLogging.CentralizedLogging.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using DotNetEnv;

namespace LoggingMicroservice.RabbitMQ
{
    public class RabbitMQConnection(IOptions<RabbitMQConfiguration> rabbitMQconfiguration) : IRabbitMQConnectionFactory
    {
        private readonly RabbitMQConfiguration _rabbitMQconfiguration = rabbitMQconfiguration.Value;

        public IConnectionFactory? Connection()
        {
            try
            {
                Env.Load();

                var rabbitUserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? string.Empty;
                var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? string.Empty;

                if (string.IsNullOrEmpty(rabbitPassword))
                    throw new InvalidOperationException("Invalid configuration, rabbitMQ password missing or empty.");

                var uri = $"amqp://{rabbitUserName}:{rabbitPassword}@{_rabbitMQconfiguration.HostName}:{_rabbitMQconfiguration.Port}";

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(uri)
                };

                return factory;
            }
            catch (Exception e)
            {
                Console.WriteLine($"RabbitMQ connection failed : {e}");
                return null;
            }
        }

    }
}