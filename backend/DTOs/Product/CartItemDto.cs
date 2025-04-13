namespace backend.DTOs.Product
{
    public class CartItemDto
    {
        public Guid id { get; set; }
        public int quantity { get; set; }
        public string ? note { get; set; }
    }
}
