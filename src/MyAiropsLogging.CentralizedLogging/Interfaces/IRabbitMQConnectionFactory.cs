using RabbitMQ.Client;

namespace LoggingMicroservice.Interfaces
{
    public interface IRabbitMQConnectionFactory
    {
        public IConnectionFactory? Connection();

    }
}