using RabbitMQ.Client;

namespace PlatformService.AsyncDataService
{
    public interface IRabbitMQConnectionFactory 
    {
        IConnection CreateConnection();
    }
}