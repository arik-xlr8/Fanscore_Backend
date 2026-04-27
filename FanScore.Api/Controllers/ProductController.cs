using System.Security.Claims;
using Fanscore.Application.DTOs.Product;
using FanScore.Api.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public ProductController(
            IProductService productService,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _productService = productService;
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Kullanıcı bilgisi alınamadı." });

                int userId = int.Parse(userIdClaim);

                var dto = new ProductCreateDto
                {
                    Name = request.Name,
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    Price = request.Price,
                    TeamId = request.TeamId,
                    CityId = request.CityId,
                    Condition = request.Condition
                };

                var photos = request.Pictures?
                    .Where(file => file.Length > 0)
                    .Select(file => new ProductPhotoUploadDto
                    {
                        FileStream = file.OpenReadStream(),
                        OriginalFileName = file.FileName
                    })
                    .ToList();

                var webRootPath = _env.WebRootPath;

                if (string.IsNullOrWhiteSpace(webRootPath))
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var baseUrl = _configuration["App:BackendBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";

                var createdProduct = await _productService.CreateAsync(
                    userId,
                    dto,
                    photos,
                    webRootPath,
                    baseUrl
                );

                return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Kullanıcı bilgisi alınamadı." });

                int userId = int.Parse(userIdClaim);

                var dto = new ProductUpdateDto
                {
                    Name = request.Name,
                    ShortDescription = request.ShortDescription,
                    Description = request.Description,
                    Price = request.Price,
                    TeamId = request.TeamId,
                    CityId = request.CityId,
                    Condition = request.Condition
                };

                var photos = request.Pictures?
                    .Where(file => file.Length > 0)
                    .Select(file => new ProductPhotoUploadDto
                    {
                        FileStream = file.OpenReadStream(),
                        OriginalFileName = file.FileName
                    })
                    .ToList();

                var webRootPath = _env.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var baseUrl = _configuration["App:BackendBaseUrl"] ?? $"{Request.Scheme}://{Request.Host}";

                var updated = await _productService.UpdateAsync(
                    id,
                    userId,
                    dto,
                    photos,
                    request.ReplacePhotos,
                    webRootPath,
                    baseUrl
                );

                if (!updated)
                    return NotFound(new { message = "Ürün bulunamadı." });

                return Ok(new { message = "Ürün başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Kullanıcı bilgisi alınamadı." });

                int userId = int.Parse(userIdClaim);

                var deleted = await _productService.DeleteAsync(id, userId);

                if (!deleted)
                    return NotFound(new { message = "Ürün bulunamadı." });

                return Ok(new { message = "Ürün başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Kullanıcı bilgisi alınamadı." });

            int userId = int.Parse(userIdClaim);

            var products = await _productService.GetByUserIdAsync(userId);

            return Ok(products);
        }
    }

    public class ProductCreateRequest
    {
        public string Name { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? TeamId { get; set; }
        public int CityId { get; set; }

        public string Condition { get; set; } = "Iyi";

        public List<IFormFile>? Pictures { get; set; }
    }

    public class ProductUpdateRequest
    {
        public string Name { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int? TeamId { get; set; }
        public int CityId { get; set; }

        public string Condition { get; set; } = "Iyi";

        public bool ReplacePhotos { get; set; } = false;

        public List<IFormFile>? Pictures { get; set; }
    }
}