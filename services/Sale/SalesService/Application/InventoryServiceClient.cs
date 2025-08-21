namespace SalesService.Application
{
    public class InventoryServiceClient
    {
        private readonly HttpClient _httpClient;

        public InventoryServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("InventoryService");
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"api/products/{productId}");
            return product != null && product.StockQuantity >= quantity;
        }

        private class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int StockQuantity { get; set; }
        }
    }
}
