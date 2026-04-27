using System;
using System.Collections.Generic;
using Fanscore.Domain.Entities;

namespace FanScore.Domain.Entities;

public partial class Halisaha
{
    public int HaliSahaId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal Price { get; set; }

    public int TeamSize { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public int CityId { get; set; }
    
    public virtual City City { get; set; } = null!;
}
