using Fanscore.Application.DTOs.Product;
using FanScore.Api.Services.Abstract;
using FanScore.Domain.Entities;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Api.Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        private const string DefaultProductImageUrl =
            "http://localhost:5153/uploads/products/default_product.png";

        public async Task<List<ProductListDto>> GetAllAsync()
        {
            return await _context.Products
                .OrderByDescending(p => p.ListedAt)
                .Select(p => new ProductListDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    ListedAt = p.ListedAt,
                    Condition = p.Condition,

                    UserId = p.UserId,
                    UserName = p.User.UserName,
                    UserProfilePic = p.User.ProfilePic,

                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.TeamName : null,

                    CityId = p.CityId,
                    CityName = p.City.CityName,

                    MainPicUrl = _context.Pics
                        .Where(x => x.ProductId == p.ProductId)
                        .Select(x => x.PicUrl)
                        .FirstOrDefault() ?? DefaultProductImageUrl
                })
                .ToListAsync();
        }

        public async Task<ProductDetailDto?> GetByIdAsync(int productId)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetailDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    Description = p.Description,
                    Price = p.Price,
                    ListedAt = p.ListedAt,
                    Condition = p.Condition,

                    UserId = p.UserId,
                    UserName = p.User.UserName,
                    UserProfilePic = p.User.ProfilePic,
                    PhoneNumber = p.User.PhoneNumber,

                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.TeamName : null,

                    CityId = p.CityId,
                    CityName = p.City.CityName,

                    Pictures = _context.Pics
                        .Where(x => x.ProductId == p.ProductId)
                        .Select(x => x.PicUrl)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product != null && product.Pictures.Count == 0)
            {
                product.Pictures.Add(DefaultProductImageUrl);
            }

            return product;
        }

        public async Task<ProductDetailDto> CreateAsync(
            int userId,
            ProductCreateDto dto,
            List<ProductPhotoUploadDto>? photos,
            string webRootPath,
            string baseUrl)
        {
            var product = new Product
            {
                Name = dto.Name,
                ShortDescription = dto.ShortDescription,
                Description = dto.Description,
                Price = dto.Price,
                ListedAt = DateTime.UtcNow,
                UserId = userId,
                TeamId = dto.TeamId,
                CityId = dto.CityId,
                Condition = dto.Condition
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var uploadPath = Path.Combine(webRootPath, "uploads", "products");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var pictureUrls = new List<string>();

            if (photos != null && photos.Any())
            {
                foreach (var photo in photos)
                {
                    var extension = Path.GetExtension(photo.OriginalFileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await photo.FileStream.CopyToAsync(stream);
                    }

                    pictureUrls.Add($"{baseUrl}/uploads/products/{fileName}");
                }
            }
            else
            {
                pictureUrls.Add($"{baseUrl}/uploads/products/default_product.png");
            }

            foreach (var url in pictureUrls)
            {
                _context.Pics.Add(new Pic
                {
                    ProductId = product.ProductId,
                    PicUrl = url
                });
            }

            await _context.SaveChangesAsync();

            var created = await GetByIdAsync(product.ProductId);
            return created!;
        }

        public async Task<List<ProductListDto>> GetByUserIdAsync(int userId)
        {
            return await _context.Products
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.ListedAt)
                .Select(p => new ProductListDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    ListedAt = p.ListedAt,
                    Condition = p.Condition,

                    UserId = p.UserId,
                    UserName = p.User.UserName,
                    UserProfilePic = p.User.ProfilePic,

                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.TeamName : null,

                    CityId = p.CityId,
                    CityName = p.City.CityName,

                    MainPicUrl = _context.Pics
                        .Where(x => x.ProductId == p.ProductId)
                        .Select(x => x.PicUrl)
                        .FirstOrDefault() ?? DefaultProductImageUrl
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(
            int productId,
            int userId,
            ProductUpdateDto dto,
            List<ProductPhotoUploadDto>? photos,
            bool replacePhotos,
            string webRootPath,
            string baseUrl)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product == null)
                return false;

            if (product.UserId != userId)
                throw new Exception("Bu ürünü güncelleme yetkiniz yok.");

            var cityExists = await _context.Cities.AnyAsync(x => x.CityId == dto.CityId);
            if (!cityExists)
                throw new Exception("Geçersiz şehir seçildi.");

            if (dto.TeamId.HasValue)
            {
                var teamExists = await _context.Teams.AnyAsync(x => x.TeamId == dto.TeamId.Value);
                if (!teamExists)
                    throw new Exception("Geçersiz takım seçildi.");
            }

            var validConditions = new[] { "Sifir", "AzKullanilmis", "Iyi", "Orta", "Yipranmis" };
            if (!validConditions.Contains(dto.Condition))
                throw new Exception("Geçersiz ürün durumu.");

            product.Name = dto.Name;
            product.ShortDescription = dto.ShortDescription;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.TeamId = dto.TeamId;
            product.CityId = dto.CityId;
            product.Condition = dto.Condition;

            if (replacePhotos)
            {
                var oldPics = await _context.Pics
                    .Where(x => x.ProductId == productId)
                    .ToListAsync();

                if (oldPics.Any())
                    _context.Pics.RemoveRange(oldPics);
            }

            if (photos != null && photos.Any())
            {
                var uploadPath = Path.Combine(webRootPath, "uploads", "products");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var photo in photos)
                {
                    var extension = Path.GetExtension(photo.OriginalFileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await photo.FileStream.CopyToAsync(stream);
                    }

                    _context.Pics.Add(new Pic
                    {
                        ProductId = productId,
                        PicUrl = $"{baseUrl}/uploads/products/{fileName}"
                    });
                }
            }

            var hasAnyPic = await _context.Pics.AnyAsync(x => x.ProductId == productId);

            if (!hasAnyPic && (photos == null || !photos.Any()))
            {
                _context.Pics.Add(new Pic
                {
                    ProductId = productId,
                    PicUrl = $"{baseUrl}/uploads/products/default_product.png"
                });
            }

            var pics = await _context.Pics
                .Where(x => x.ProductId == productId)
                .ToListAsync();

            if (pics.Count > 1)
            {
                var defaultPic = pics.FirstOrDefault(x =>
                    x.PicUrl.Contains("default_product.png"));

                if (defaultPic != null)
                {
                    _context.Pics.Remove(defaultPic);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int productId, int userId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product == null)
                return false;

            if (product.UserId != userId)
                throw new Exception("Bu ürünü silme yetkiniz yok.");

            var pics = await _context.Pics
                .Where(x => x.ProductId == productId)
                .ToListAsync();

            if (pics.Any())
                _context.Pics.RemoveRange(pics);

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}