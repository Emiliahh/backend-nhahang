﻿using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Product
{

    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Image { get; set; }

    public string? CategoryId { get; set; }

    public string? Description { get; set; }

    public bool isDeleted { get; set; } = false;

    public virtual ICollection<Cartitem> Cartitems { get; set; } = [];

    public virtual Category? Category { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = [];
}
