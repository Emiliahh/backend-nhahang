namespace backend.DTOs.Product
{
    public class CartDisplayDto
    {
        public Guid id { get; set; }
        public float price { get; set; }
        public string? name { get; set; }
    }
}
