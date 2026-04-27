using Fanscore.Application.DTOs.Product;

namespace FanScore.Api.Services.Abstract
{
    public interface IProductService
    {
        Task<List<ProductListDto>> GetAllAsync();
        Task<ProductDetailDto?> GetByIdAsync(int productId);
        Task<ProductDetailDto> CreateAsync(
            int userId,
            ProductCreateDto dto,
            List<ProductPhotoUploadDto>? photos,
            string webRootPath,
            string baseUrl
        );
        Task<bool> UpdateAsync(
            int productId,
            int userId,
            ProductUpdateDto dto,
            List<ProductPhotoUploadDto>? photos,
            bool replacePhotos,
            string webRootPath,
            string baseUrl
        );
        Task<bool> DeleteAsync(int productId, int userId);
        Task<List<ProductListDto>> GetByUserIdAsync(int userId);
    }
}