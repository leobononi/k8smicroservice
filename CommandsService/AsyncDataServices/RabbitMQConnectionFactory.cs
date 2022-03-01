using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace CommandsService.AsyncDataServices
{
    public class RabbitMQConnectionFactory : IRabbitMQConnectionFactory
    {
        private readonly IConfiguration _config;

        public RabbitMQConnectionFactory(IConfiguration config)
        {
            _config = config;
        }
        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };
            
            var connection = factory.CreateConnection();
            return connection;
        }
    }
}