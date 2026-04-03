using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.User
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePic { get; set; }

        public List<MyRecentRatingDto> RecentRatings { get; set; } = new();
    }
}