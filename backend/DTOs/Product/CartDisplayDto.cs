namespace backend.DTOs.Product
{
    public class CartDisplayDto
    {
        public Guid id { get; set; }
        public decimal price { get; set; }
        public string? name { get; set; }
    }
}
