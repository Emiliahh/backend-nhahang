using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Product
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public float? Price { get; set; }

    public string? Image { get; set; }

    public string? CategoryId { get; set; }

    public string? Description { get; set; }

    public bool isDeleted { get; set; } = false;

    public virtual ICollection<Cartitem> Cartitems { get; set; } = new List<Cartitem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();
}
