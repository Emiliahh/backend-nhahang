using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Orderdetail
{
    public string Id { get; set; } = null!;

    public string? OrderId { get; set; }

    public string? ProductId { get; set; }

    public int? Quantity { get; set; }

    public string? Note { get; set; }

    public virtual Foodorder? Order { get; set; }

    public virtual Product? Product { get; set; }
}
