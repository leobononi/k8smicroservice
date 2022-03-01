using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IEventProcessor _enventProcessor;

        public IRabbitMQEventConsumer _eventConsumer { get; }

        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(
            IRabbitMQConnectionFactory connFactory, 
            IEventProcessor eventProcessor,
            IRabbitMQEventConsumer eventConsumer)
        {
            _enventProcessor = eventProcessor;
            _eventConsumer = eventConsumer;

            InitializeRabbitMQ(connFactory);
        }

        private void InitializeRabbitMQ(IRabbitMQConnectionFactory connFactory)
        {
             try
            {
                _connection = connFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(
                    queue: _queueName,
                    exchange: "trigger",
                    routingKey: "");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("--> Connected to the message bus");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }   
        }

        public override void Dispose()
        {
            Console.WriteLine("Messagebus Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = _eventConsumer.CreateConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("--> Event Received");

                var notificationMessage = Encoding.UTF8.GetString(ea.Body.ToArray());

                _enventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}