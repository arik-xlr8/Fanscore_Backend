using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Rating
{
    public class RatingReactionResultDto
    {
        public int RatingId { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string UserReaction { get; set; } = null!;
    }
}