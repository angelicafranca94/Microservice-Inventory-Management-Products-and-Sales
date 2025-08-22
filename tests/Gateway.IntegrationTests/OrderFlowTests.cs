using FluentAssertions;
using InventoryService.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SalesService.DTOs;
using System.Net.Http.Json;

namespace Gateway.IntegrationTests;

public class OrderFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrderFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateOrder_ThroughGateway_Should_UpdateStock()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Obter token JWT
        var token = await AuthHelper.GetJwtToken(client, "admin", "senha123");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Criar produto via Gateway -> StockService
        var productDto = new ProductCreateDto
        {
            Name = "Produto Gateway",
            Description = "Teste E2E",
            Price = 50,
            Quantity = 10
        };

        var productResponse = await client.PostAsJsonAsync("/api/products", productDto);
        productResponse.EnsureSuccessStatusCode();
        var createdProduct = await productResponse.Content.ReadFromJsonAsync<ProductReadDto>();

        // Criar pedido via Gateway -> SalesService
        var orderDto = new OrderCreateDto
        {
            Items = new List<OrderItemDto>
            {
                new() { ProductId = createdProduct.Id, Quantity = 3 }
            }
        };

        var orderResponse = await client.PostAsJsonAsync("/api/orders", orderDto);
        orderResponse.EnsureSuccessStatusCode();

        // Consultar estoque via Gateway -> StockService
        var stockResponse = await client.GetAsync($"/api/products/{createdProduct.Id}");
        stockResponse.EnsureSuccessStatusCode();
        var updatedProduct = await stockResponse.Content.ReadFromJsonAsync<ProductReadDto>();

        // Assert
        updatedProduct.Quantity.Should().Be(7); // 10 - 3 = 7
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldFail()
    {
        var client = _factory.CreateClient();
        var token = await AuthHelper.GetJwtToken(client, "admin", "senha123");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Produto com estoque 2
        var productDto = new ProductCreateDto
        {
            Name = "Produto Insuficiente",
            Description = "Teste Falha",
            Price = 20,
            Quantity = 2
        };

        var productResponse = await client.PostAsJsonAsync("/api/products", productDto);
        productResponse.EnsureSuccessStatusCode();
        var product = await productResponse.Content.ReadFromJsonAsync<ProductReadDto>();

        // Pedido de quantidade 5
        var orderDto = new OrderCreateDto
        {
            Items = new List<OrderItemDto> { new() { ProductId = product.Id, Quantity = 5 } }
        };

        var orderResponse = await client.PostAsJsonAsync("/api/orders", orderDto);

        // Verifica se o pedido não foi aceito
        orderResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var stockResponse = await client.GetAsync($"/api/products/{product.Id}");
        var updatedProduct = await stockResponse.Content.ReadFromJsonAsync<ProductReadDto>();

        updatedProduct.Quantity.Should().Be(2); // Estoque não foi alterado
    }
}