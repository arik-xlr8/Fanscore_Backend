using System;
using System.Collections.Generic;

namespace FanScore.Domain.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTime ListedAt { get; set; }

    public int UserId { get; set; }

    public int? PicId { get; set; }

    public virtual Pic? Pic { get; set; }

    public virtual ICollection<Pic> Pics { get; set; } = new List<Pic>();

    public virtual User User { get; set; } = null!;
}
