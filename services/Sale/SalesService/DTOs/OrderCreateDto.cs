using System.ComponentModel.DataAnnotations;

namespace SalesService.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public IEnumerable<OrderItemDto> Items { get; set; } = [];
    }
}
