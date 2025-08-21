namespace SalesService.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemReadDto> Items { get; set; }
    }

}
