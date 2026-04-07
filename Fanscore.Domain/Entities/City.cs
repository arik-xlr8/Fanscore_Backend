using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FanScore.Domain.Entities;

namespace Fanscore.Domain.Entities
{
    public class City
    {
        public int CityId { get; set; }
        public int PlateCode { get; set; }
        public string CityName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}