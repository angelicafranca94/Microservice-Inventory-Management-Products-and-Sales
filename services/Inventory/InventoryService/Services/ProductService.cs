using InventoryService.DTOs;
using InventoryService.Models;
using InventoryService.Repositories;

namespace InventoryService.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Quantity = p.Quantity,
                Price = p.Price
            });
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price
            };
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            await _repository.AddAsync(product);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price
            };
        }

        public async Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            if (id != dto.Id || !await _repository.ExistsAsync(id))
                return false;

            var product = new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Price = dto.Price
            };

            await _repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            await _repository.DeleteAsync(product);
            return true;
        }
    }
}
