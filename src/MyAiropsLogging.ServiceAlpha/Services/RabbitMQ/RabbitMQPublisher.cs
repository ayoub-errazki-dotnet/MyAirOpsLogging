using MassTransit.Configuration;
using RabbitMQ.Client;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using MassTransit.SagaStateMachine;
using LoggingMicroservice.Interfaces;

namespace LoggingMicroservice.RabbitMQ
{
    public class RabbitMQPublisher<T>(IRabbitMQConnectionFactory factory) : IRabbitMQPublisher<T>
    {
        private readonly IRabbitMQConnectionFactory _factory = factory;

        public async Task PublishMessageAsync(T message, string queueName)
        {
            try
            {
                var connection = await _factory.Connection().CreateConnectionAsync() 
                                ?? throw new NullReferenceException("RabbitMQ connection failed");                

                using var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                string messageJson = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(messageJson);

                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);

                Console.WriteLine("[x] Sent {0}", message);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing the message : {ex.Message}");
            }
        }
    }
}
