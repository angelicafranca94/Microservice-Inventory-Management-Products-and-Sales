using InventoryService.Data;
using MassTransit;
using Serilog;
using Shared.Messages;

namespace InventoryService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly InventoryDbContext _db;

    public OrderCreatedConsumer(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        Log.Information("Recebendo evento OrderCreated para OrderId {OrderId}", context.Message.OrderId);

        foreach (var item in context.Message.Items)
        {
            var product = await _db.Products.FindAsync(item.ProductId);

            if (product != null)
            {
                if (product.Quantity >= item.Quantity)
                {
                    product.Quantity -= item.Quantity;
                    Log.Information(
                        "Produto {ProductId} atualizado. Quantidade anterior: {OldQuantity}, nova quantidade: {NewQuantity}",
                        product.Id,
                        product.Quantity + item.Quantity,
                        product.Quantity
                    );
                }
                else
                {
                    Log.Warning(
                        "Estoque insuficiente para Produto {ProductId}. Estoque atual: {Quantity}, pedido: {OrderQuantity}",
                        product.Id,
                        product.Quantity,
                        item.Quantity
                    );
                }
            }
            else
            {
                Log.Warning("Produto {ProductId} não encontrado no estoque", item.ProductId);
            }
        }

        await _db.SaveChangesAsync();
        Log.Information("Estoque atualizado com sucesso para OrderId {OrderId}", context.Message.OrderId);
    }
}