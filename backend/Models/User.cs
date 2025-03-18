using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backend.Models;

[Index(nameof(Email), IsUnique = true)]
public partial class User: IdentityUser<Guid>
{
    [Required]
    public string Phone { get; set; } = null!;

    public string ? Address { get; set; }

    public virtual ICollection<Cartitem> Cartitems { get; set; } = new List<Cartitem>();

    public virtual ICollection<Foodorder> Foodorders { get; set; } = new List<Foodorder>();
}
