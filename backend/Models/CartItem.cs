using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Cartitem
{
    public Guid ? UserId { get; set; }

    public Guid ProductId { get; set; } 

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
