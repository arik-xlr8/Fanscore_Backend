using Fanscore.Application.DTOs.Product;

namespace FanScore.Api.Services.Abstract
{
    public interface IProductService
    {
        Task<List<ProductListDto>> GetAllAsync();
        Task<ProductDetailDto?> GetByIdAsync(int productId);
        Task<ProductDetailDto> CreateAsync(int userId, ProductCreateDto dto);
        Task<bool> UpdateAsync(int productId, int userId, ProductUpdateDto dto);
        Task<bool> DeleteAsync(int productId, int userId);
    }
}