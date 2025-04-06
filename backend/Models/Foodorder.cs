using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Foodorder
{
    public string Id { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string CheckOutType { get; set; } = null!;

    public Guid? UserId { get; set; }

    public string? Status { get; set; }

    public DateTime DeliveryTime { get; set; }

    public string? Note { get; set; }

    public virtual Orderdetail? Orderdetail { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
public enum OrderStatus
{
    Pending =1,
    Delivering = 2,
    Delivered = 3,
    Cancel = 4,
}