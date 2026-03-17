using System;
using System.Collections.Generic;

namespace FanScore.Domain.Entities;

public partial class Halisaha
{
    public int HaliSahaId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public string City { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
