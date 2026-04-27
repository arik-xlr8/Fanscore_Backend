using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanscore.Application.DTOs.Product
{
    public class ProductPhotoUploadDto
    {
        public Stream FileStream { get; set; } = null!;
        public string OriginalFileName { get; set; } = null!;
    }
}