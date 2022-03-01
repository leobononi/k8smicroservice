using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public interface IRabbitMQEventConsumer
    {
        EventingBasicConsumer CreateConsumer(IModel model);
    }
}