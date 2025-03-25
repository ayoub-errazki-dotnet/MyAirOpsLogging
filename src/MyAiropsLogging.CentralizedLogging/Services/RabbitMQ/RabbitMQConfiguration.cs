using RabbitMQ.Client;

namespace MyAiropsLogging.CentralizedLogging.RabbitMQ
{
    public class RabbitMQConfiguration
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? PassWord { get; set; }
        public int Port { get; set; }
    }
    public static class RabbitQueues
    {
        public const string LogsQ = "LogsQ";
    }
}