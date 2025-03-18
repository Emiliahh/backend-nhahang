using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Payment
{
    public string Id { get; set; } = null!;

    public string? OrderId { get; set; }

    public float? Amount { get; set; }

    public string? TransactionCode { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Foodorder? Order { get; set; }
}
