using System;
using System.Collections.Generic;

namespace FanScore.Domain.Entities;

public partial class Pic
{
    public int PicId { get; set; }

    public int ProductId { get; set; }

    public string PicUrl { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
