using RabbitMQ.Client;

namespace InventoryService.Infrastructure
{
    public class RabbitMQConnection
    {
        private readonly IConnection _connection;

        public RabbitMQConnection(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
        }

        public IModel CreateChannel()
        {
            return _connection.CreateModel();
        }
    }
}
