namespace backend.DTOs
{
    public class PayLinkDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
