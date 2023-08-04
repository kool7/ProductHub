namespace ProductHub.Application.Contracts.Products
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Units { get; set; }
    }
}
