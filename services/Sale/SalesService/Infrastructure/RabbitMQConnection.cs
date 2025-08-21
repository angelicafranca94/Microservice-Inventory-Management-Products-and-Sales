using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace SalesService.Infrastructure
{
    public class RabbitMQConnection
    {
        private readonly IConnection _connection;

        public RabbitMQConnection(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
        }

        public RabbitMQ.Client.IModel CreateChannel()
        {
            return _connection.CreateModel();
        }
    }
}
