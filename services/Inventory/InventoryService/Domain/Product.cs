namespace InventoryService.Domain
{
    public class Product
    {
        public int Id { get; set; }    // Identificador único
        public string Name { get; set; } = "";  // Nome do produto
        public string Description { get; set; } = ""; // Descrição opcional
        public decimal Price { get; set; }      // Preço do produto
        public int Stock { get; set; }          // Quantidade em estoque
    }
}
