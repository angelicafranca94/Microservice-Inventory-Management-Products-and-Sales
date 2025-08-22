using InventoryService.Data;
using InventoryService.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Tests;

public class ProductTests
{
    private InventoryDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InventoryDbContext(options);
    }

    [Fact]
    public void Can_Add_Product()
    {
        var db = GetDbContext();
        var product = new Product { Id = 123, Name = "Produto A", Price = 10, Quantity = 5 };
        db.Products.Add(product);
        db.SaveChanges();

        Assert.Single(db.Products);
        Assert.Equal("Produto A", db.Products.First().Name);
    }
}
