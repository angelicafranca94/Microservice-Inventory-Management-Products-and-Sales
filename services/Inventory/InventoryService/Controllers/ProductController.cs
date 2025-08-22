using InventoryService.Data;
using InventoryService.DTOs;
using InventoryService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace StockService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly InventoryDbContext _db;

    public ProductsController(InventoryDbContext db)
    {
        _db = db;
    }

    // GET: api/products
    [HttpGet]
    public IActionResult GetAll()
    {
        Log.Information("Consultando todos os produtos");
        var products = _db.Products
            .Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity
            })
            .ToList();

        return Ok(products);
    }

    // GET: api/products/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        Log.Information("Consultando produto {ProductId}", id);
        var product = _db.Products.Find(id);

        if (product == null)
        {
            Log.Warning("Produto {ProductId} não encontrado", id);
            return NotFound();
        }

        var dto = new ProductReadDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        };

        return Ok(dto);
    }

    // POST: api/products
    [HttpPost]
    public IActionResult Create([FromBody] ProductCreateDto dto)
    {
        Log.Information("Criando produto {ProductName}", dto.Name);

        var product = new Product
        {
            //Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Quantity = dto.Quantity
        };

        _db.Products.Add(product);
        _db.SaveChanges();

        Log.Information("Produto {ProductId} criado com sucesso", product.Id);

        var readDto = new ProductReadDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        };

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, readDto);
    }

    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] ProductCreateDto dto)
    {
        Log.Information("Atualizando produto {ProductId}", id);

        var product = _db.Products.Find(id);
        if (product == null)
        {
            Log.Warning("Produto {ProductId} não encontrado para atualização", id);
            return NotFound();
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;

        _db.SaveChanges();
        Log.Information("Produto {ProductId} atualizado com sucesso", id);

        return NoContent();
    }

    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        Log.Information("Excluindo produto {ProductId}", id);

        var product = _db.Products.Find(id);
        if (product == null)
        {
            Log.Warning("Produto {ProductId} não encontrado para exclusão", id);
            return NotFound();
        }

        _db.Products.Remove(product);
        _db.SaveChanges();

        Log.Information("Produto {ProductId} excluído com sucesso", id);

        return NoContent();
    }
}