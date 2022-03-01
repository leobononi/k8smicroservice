using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class RabbitMQEventConsumer : IRabbitMQEventConsumer
    {
        public EventingBasicConsumer CreateConsumer(IModel model)
        {
            return new EventingBasicConsumer(model);
        }
    }
}