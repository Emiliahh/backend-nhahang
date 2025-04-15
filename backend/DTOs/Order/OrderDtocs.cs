namespace backend.DTOs.Order
{
    public class CreateOrderDto
    {
        public PaymentMethodType PaymentMethod { get; set; }

        public string? Address { get; set; }

        public decimal DeliveryFee { get; set; }

        public string? PromoCode { get; set; } 

        public string? Note { get; set; }

        public List<CreateOrderDetailDto> CreateOrderDetailDtos { get; set; } = [];
    }

    public class CreateOrderDetailDto
    {

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public string? Note { get; set; }

    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public Guid? UserId { get; set; }
        public OrderStatus Status { get; set; }
        public string? Address { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal DeliveryFee { get; set; }
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public string? InternalNote { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string? Note { get; set; }
        public string CustomerName { get; set; } = null!;
        public List<OrderDetailDto> OrderDetails { get; set; } = [];
    }
    public class OrderDetailDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public int Status { get; set; }
    }

}
