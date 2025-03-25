using LoggingMicroservice.Interfaces;
using MyAiropsLogging.CentralizedLogging.Interfaces;

using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

namespace LoggingMicroservice.RabbitMQ
{
    public class RabbitMQPublisher<T>(IRabbitMQConnectionFactory factory) : IRabbitMQPublisher<T>
    {
        private readonly IRabbitMQConnectionFactory _factory = factory;

        public async Task PublishMessageAsync(T message, string queueName)
        {
            try
            {
                var connectionFactory = _factory.Connection();
                if (connectionFactory == null)
                    throw new NullReferenceException("RabbitMQ connection factory is null");


                var connection = await connectionFactory.CreateConnectionAsync()
                                ?? throw new NullReferenceException("RabbitMQ connection failed");

                using var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var props = new BasicProperties();
                string messageJson = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(messageJson);
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;

                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, true, basicProperties: props, body: body);

                Console.WriteLine("[x] Sent {0}", message);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing the message : {ex.Message}");
            }
        }
    }
}
