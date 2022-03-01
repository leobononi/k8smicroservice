using RabbitMQ.Client;

namespace CommandsService.AsyncDataServices
{
    public interface IRabbitMQConnectionFactory
    {
        IConnection CreateConnection();
    }
}