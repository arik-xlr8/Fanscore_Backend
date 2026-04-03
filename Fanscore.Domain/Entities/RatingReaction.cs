using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanScore.Domain.Entities;

namespace Fanscore.Domain.Entities
{
    public class RatingReaction
    {
        public int RatingReactionId { get; set; }
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public string ReactionType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual Rating Rating { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}