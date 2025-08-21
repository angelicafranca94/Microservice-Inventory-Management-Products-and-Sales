namespace InventoryService.Application.Events
{
    public class ProductCreatedEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int Stock { get; set; }
    }
}
