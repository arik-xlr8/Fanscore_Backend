using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Product
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime ListedAt { get; set; }

        public string? Condition { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserProfilePic { get; set; }
        public string? PhoneNumber { get; set; }

        public int? TeamId { get; set; }
        public string? TeamName { get; set; }

        public int CityId { get; set; }
        public string? CityName { get; set; }

        public List<string> Pictures { get; set; } = new();
    }
}