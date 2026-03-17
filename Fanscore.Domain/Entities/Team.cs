using System;
using System.Collections.Generic;

namespace FanScore.Domain.Entities;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
