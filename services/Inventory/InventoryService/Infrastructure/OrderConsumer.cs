using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventoryService.Infrastructure
{
    public class OrderConsumer
    {
        private class OrderItemMessage
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        private readonly RabbitMQConnection _rabbitConnection;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderConsumer(RabbitMQConnection rabbitConnection, ILogger logger, IServiceScopeFactory scopeFactory)
        {
            _rabbitConnection = rabbitConnection;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public void Start()
        {
            var channel = _rabbitConnection.CreateChannel();
            channel.QueueDeclare(queue: "order_created", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received order message: {Message}", message);

                var items = JsonSerializer.Deserialize<List<OrderItemMessage>>(message);

                // Cria scope para usar DbContext scoped
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

                    foreach (var item in items)
                    {
                        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            product.Stock -= item.Quantity;
                            _logger.LogInformation("Updated stock for product {ProductId}: {NewStock}", product.Id, product.Stock);
                        }
                    }

                    await context.SaveChangesAsync();
                }
            };

            channel.BasicConsume(queue: "order_created", autoAck: true, consumer: consumer);
        }
    }
}