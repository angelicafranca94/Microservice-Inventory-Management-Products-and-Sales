using InventoryService.DTOs;

namespace InventoryService.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<bool> UpdateAsync(int id, ProductDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
