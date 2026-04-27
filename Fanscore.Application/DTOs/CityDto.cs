using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs
{
    public class CityDto
    {
        public int CityId { get; set; }
        public int PlateCode { get; set; }
        public string CityName { get; set; } = null!;

    }
}