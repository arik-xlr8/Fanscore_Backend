using System;
using System.Collections.Generic;

namespace FanScore.Domain.Entities;

public partial class Player
{
    public int PlayerId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int? TeamId { get; set; }

    public int? Age { get; set; }

    public string? Position { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual Team? Team { get; set; }
}
