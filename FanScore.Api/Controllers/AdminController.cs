using FanScore.Application.DTOs.Admin;
using FanScore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanScore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("panel")]
        public async Task<IActionResult> GetPanelData()
        {
            return Ok(await _adminService.GetPanelDataAsync());
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            return Ok(await _adminService.GetDashboardStatsAsync());
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _adminService.GetUsersAsync());
        }

        [HttpPut("users/{userId}/ban")]
        public async Task<IActionResult> BanUser(int userId, AdminBanUserDto dto)
        {
            var result = await _adminService.BanUserAsync(userId, dto.BanReason);
            if (!result)
                return NotFound(new { message = "Kullanici bulunamadi." });

            return Ok(new { message = "Kullanici banlandi." });
        }

        [HttpPut("users/{userId}/unban")]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            var result = await _adminService.UnbanUserAsync(userId);
            if (!result)
                return NotFound(new { message = "Kullanici bulunamadi." });

            return Ok(new { message = "Kullanici bani kaldirildi." });
        }

        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> ChangeUserRole(int userId, AdminChangeRoleDto dto)
        {
            try
            {
                var result = await _adminService.ChangeUserRoleAsync(userId, dto.Role);
                if (!result)
                    return NotFound(new { message = "Kullanici bulunamadi." });

                return Ok(new { message = "Rol guncellendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("products/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, AdminUpdateProductDto dto)
        {
            try
            {
                var result = await _adminService.UpdateProductAsync(productId, dto);
                if (!result)
                    return NotFound(new { message = "Urun bulunamadi." });

                return Ok(new { message = "Urun guncellendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("products/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var result = await _adminService.DeleteProductAsync(productId);
            if (!result)
                return NotFound(new { message = "Urun bulunamadi." });

            return Ok(new { message = "Urun silindi." });
        }

        [HttpPut("halisahas/{haliSahaId}")]
        public async Task<IActionResult> UpdateHalisaha(int haliSahaId, AdminUpdateHalisahaDto dto)
        {
            try
            {
                var result = await _adminService.UpdateHalisahaAsync(haliSahaId, dto);
                if (!result)
                    return NotFound(new { message = "Hali saha ilani bulunamadi." });

                return Ok(new { message = "Hali saha ilani guncellendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("halisahas/{haliSahaId}")]
        public async Task<IActionResult> DeleteHalisaha(int haliSahaId)
        {
            var result = await _adminService.DeleteHalisahaAsync(haliSahaId);
            if (!result)
                return NotFound(new { message = "Hali saha ilani bulunamadi." });

            return Ok(new { message = "Hali saha ilani silindi." });
        }
    }
}
