using SalesService.Domain;
using SalesService.Infrastructure;
using System.Text;
using System.Text.Json;

namespace SalesService.Application
{
    public class OrderEventPublisher
    {
        private readonly RabbitMQConnection _rabbitConnection;

        public OrderEventPublisher(RabbitMQConnection rabbitConnection)
        {
            _rabbitConnection = rabbitConnection;
        }

        public void PublishOrderCreated(Order order)
        {
            using var channel = _rabbitConnection.CreateChannel();

            channel.QueueDeclare(queue: "order_created",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var message = JsonSerializer.Serialize(order.Items.Select(i => new
            {
                i.ProductId,
                i.Quantity
            }));

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "order_created",
                                 mandatory: false,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
