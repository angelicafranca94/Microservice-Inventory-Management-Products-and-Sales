using System.ComponentModel.DataAnnotations;

namespace SalesService.DTOs
{
    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
