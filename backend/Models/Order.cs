using backend.Models;

public partial class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public PaymentMethodType PaymentMethod { get; set; }

    public Guid? UserId { get; set; }

    public OrderStatus Status { get; set; }

    public string? Address { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? Discount { get; set; } 

    public decimal DeliveryFee { get; set; }

    public decimal ProductTotal { get; set; }

    public bool IsPaid { get; set; } = false;

    public DateTime OrderTime { get; set; } = DateTime.Now;

    public string? InternalNote { get; set; }

    public DateTime? DeliveryTime { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = [];

    public virtual User? User { get; set; }
}

public enum OrderStatus
{
    Pending = 1,
    Delivering = 2,
    Delivered = 3,
    Cancel = 4,
}

public enum PaymentMethodType
{
    Cash = 1,
    QR_Pay = 2,
}
