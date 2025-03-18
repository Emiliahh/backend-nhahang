namespace backend.DTOs.Product
{
    public class CartDisplayDto
    {
        public string id { get; set; } = null!;
        public float price { get; set; }
        public string? name { get; set; }
    }
}
