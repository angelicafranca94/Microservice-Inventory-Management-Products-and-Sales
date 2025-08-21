using Microsoft.EntityFrameworkCore;
using SalesService.Application.Exceptions;
using SalesService.Domain;
using SalesService.DTOs;
using SalesService.Infrastructure;

namespace SalesService.Application
{
    public class OrderService
    {
        private readonly SalesContext _context;
        private readonly InventoryServiceClient _stockClient;
        private readonly OrderEventPublisher _eventPublisher;
        private readonly ILogger<OrderService> _logger;

        public OrderService(SalesContext context, InventoryServiceClient stockClient,
            OrderEventPublisher eventPublisher, ILogger<OrderService> logger)
        {
            _context = context;
            _stockClient = stockClient;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<OrderReadDto> CreateOrderAsync(OrderCreateDto dto)
        {
            _logger.LogInformation("Starting order creation for {ItemCount} items.", dto.Items.Count);

            foreach (var item in dto.Items)
            {
                bool available = await _stockClient.IsStockAvailableAsync(item.ProductId, item.Quantity);
                if (!available)
                {
                    _logger.LogWarning("Product {ProductId} does not have enough stock.", item.ProductId);
                    throw new StockUnavailableException($"Product {item.ProductId} does not have enough stock.");
                }
            }

            var order = new Order { TotalAmount = 0 };

            foreach (var itemDto in dto.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    ProductName = $"Product {itemDto.ProductId}",
                    Quantity = itemDto.Quantity,
                    Price = 10m
                };

                order.TotalAmount += orderItem.Price * orderItem.Quantity;
                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _eventPublisher.PublishOrderCreated(order);
            _logger.LogInformation("Order {OrderId} created successfully.", order.Id);

            return new OrderReadDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(i => new OrderItemReadDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}