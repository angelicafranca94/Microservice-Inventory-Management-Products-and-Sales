namespace SalesService.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IEnumerable<OrderItem> Items { get; set; } = [];
        public string Status { get; set; } = "Pending";
    }
}
