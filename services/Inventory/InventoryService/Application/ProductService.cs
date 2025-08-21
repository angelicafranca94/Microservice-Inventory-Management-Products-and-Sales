using InventoryService.Application.Events;
using InventoryService.Domain;
using InventoryService.DTOs;
using InventoryService.Infrastructure;
using InventoryService.Messaging;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application
{
    public class ProductService
    {
        private readonly InventoryDbContext _context;

        public ProductService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<ProductReadDto> AddProductAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.Stock
            };
        }

        public async Task<List<ProductReadDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.Stock
                }).ToListAsync();
        }
    }
}