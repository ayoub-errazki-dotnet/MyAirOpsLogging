using RabbitMQ.Client;

namespace MyAiropsLogging.CentralizedLogging.Interfaces
{
    public interface IRabbitMQConnectionFactory
    {
        public IConnectionFactory? Connection();

    }
}