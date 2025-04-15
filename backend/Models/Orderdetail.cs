using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Orderdetail
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
