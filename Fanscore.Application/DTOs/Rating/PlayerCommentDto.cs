using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Rating
{
    public class PlayerCommentDto
    {
        public int RatingId { get; set; }
        public string UserName { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}