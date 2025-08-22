namespace Shared.Messages;

public record OrderCreated(int OrderId, List<OrderItemEvent> Items);

public record OrderItemEvent(int ProductId, int Quantity);