namespace backend.DTOs.Product
{
    public class CartItemDto
    {
        public string id { get; set; } = null!;
        public int quantity { get; set; }
        public string ? note { get; set; }
    }
}
