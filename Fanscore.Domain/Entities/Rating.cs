using System;
using System.Collections.Generic;
using Fanscore.Domain.Entities;

namespace FanScore.Domain.Entities;

public partial class Rating
{
    public int RatingId { get; set; }

    public int UserId { get; set; }

    public int PlayerId { get; set; }

    public decimal RatingValue { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public string PeriodType { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public int LikeCount { get; set; }

    public int DislikeCount { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<RatingReaction> RatingReactions { get; set; } = new List<RatingReaction>();
}
