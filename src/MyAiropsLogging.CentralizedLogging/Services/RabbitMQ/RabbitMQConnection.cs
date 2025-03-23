using System.Threading.Tasks;
using LoggingMicroservice.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using DotNetEnv;

namespace LoggingMicroservice.RabbitMQ
{
    public class RabbitMQConnection(IOptions<RabbitMQConfiguration> rabbitMQconfiguration, IConfiguration configuration, IHostEnvironment env) : IRabbitMQConnectionFactory
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly RabbitMQConfiguration _rabbitMQconfiguration = rabbitMQconfiguration.Value;
        private readonly IHostEnvironment _env = env;

        public IConnectionFactory? Connection()
        {
            try
            {
                Env.Load();

                var rabbitUserName = string.Empty;
                var rabbitPassword = string.Empty;
                
                if (_env.IsDevelopment()){
                    rabbitUserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME")?? string.Empty;
                    rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD")?? string.Empty;
                }
                else
                    rabbitPassword = Environment.GetEnvironmentVariable("RabbitMqPassword") ?? "";
                    
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