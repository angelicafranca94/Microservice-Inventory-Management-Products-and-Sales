using Microsoft.EntityFrameworkCore;
using SalesService.Data;
using SalesService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesService.Test;

public class OrderTests
{
    private SalesDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<SalesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SalesDbContext(options);
    }

    [Fact]
    public void Can_Create_Order()
    {
        var db = GetDbContext();
        var order = new Order
        {
            Id = 123,
            Items = new List<OrderItem> { new() { ProductId = 123, Quantity = 2 } }
        };
        db.Orders.Add(order);
        db.SaveChanges();

        Assert.Single(db.Orders);
        Assert.Single(db.Orders.First().Items);
    }


}