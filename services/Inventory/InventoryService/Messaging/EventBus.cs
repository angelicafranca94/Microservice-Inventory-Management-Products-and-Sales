using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace InventoryService.Messaging
{
    public class EventBus
    {
        private readonly string _hostname;
        private readonly string _queueName;

        public EventBus(string hostname, string queueName)
        {
            _hostname = hostname;
            _queueName = queueName;
        }

        public void Publish<T>(T @event)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };

            // Conexão e canal síncronos
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declara a fila caso não exista
            channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Serializa o evento e publica
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);
        }
    }
}