using FanScore.Application.DTOs.Admin;

namespace FanScore.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardStatsDto> GetDashboardStatsAsync();
        Task<AdminPanelDto> GetPanelDataAsync();
        Task<List<AdminUserDto>> GetUsersAsync();
        Task<bool> BanUserAsync(int userId, string? banReason);
        Task<bool> UnbanUserAsync(int userId);
        Task<bool> ChangeUserRoleAsync(int userId, string role);
        Task<bool> UpdateProductAsync(int productId, AdminUpdateProductDto dto);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> UpdateHalisahaAsync(int haliSahaId, AdminUpdateHalisahaDto dto);
        Task<bool> DeleteHalisahaAsync(int haliSahaId);
    }
}
