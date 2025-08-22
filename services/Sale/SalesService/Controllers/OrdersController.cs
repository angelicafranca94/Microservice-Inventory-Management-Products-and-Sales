using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.Data;
using SalesService.DTOs;
using SalesService.Entities;
using Serilog;
using Shared.Messages;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly SalesDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(SalesDbContext db, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
        }

        // GET: api/orders
        [HttpGet]
        public IActionResult GetAll()
        {
            Log.Information("Consultando todos os pedidos");
            var orders = _db.Orders
                .Select(o => new
                {
                    o.Id,
                    o.Status,
                    Items = o.Items.Select(i => new { i.ProductId, i.Quantity })
                })
                .ToList();

            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            Log.Information("Consultando pedido {OrderId}", id);

            var order = _db.Orders.Find(id);
            if (order == null)
            {
                Log.Warning("Pedido {OrderId} não encontrado", id);
                return NotFound();
            }

            var dto = new
            {
                order.Id,
                order.Status,
                Items = order.Items.Select(i => new { i.ProductId, i.Quantity })
            };

            return Ok(dto);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            Log.Information("Criando pedido com {ItemCount} itens", dto.Items.Count());

            var order = new Order
            {
               // Id = Guid.NewGuid(),
                Status = "Pending",
                Items = dto.Items.Select(i => new Entities.OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            Log.Information("Pedido {OrderId} criado com sucesso", order.Id);

            // Publica evento para StockService via RabbitMQ
            var orderCreatedEvent = new OrderCreated(order.Id, order.Items.Select(i => new OrderItemEvent(i.ProductId, i.Quantity)).ToList());

            try
            {
                await _publishEndpoint.Publish(orderCreatedEvent);
                Log.Information("Evento OrderCreated publicado para pedido {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao publicar evento OrderCreated para pedido {OrderId}", order.Id);
            }

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
    }
}