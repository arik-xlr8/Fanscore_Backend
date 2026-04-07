using System;
using System.Collections.Generic;
using Fanscore.Domain.Entities;

namespace FanScore.Domain.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ShortDescription { get; set; }

    public int CityId { get; set; }

    public string Condition { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime ListedAt { get; set; }

    public int UserId { get; set; }

    public int? TeamId { get; set; }

    public int? PicId { get; set; }

    public virtual Pic? Pic { get; set; }

    public virtual ICollection<Pic> Pics { get; set; } = new List<Pic>();

    public virtual Team? Team { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
