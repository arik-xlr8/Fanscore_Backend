using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Product
{
    public class ProductUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? TeamId { get; set; }
        public int CityId { get; set; }

        public string Condition { get; set; } = "Iyi";

    }
}