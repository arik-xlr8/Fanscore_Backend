using Fanscore.Application.DTOs.Product;
using Fanscore.Application.DTOs.Tournament;
using Fanscore.Application.DTOs.Tournement;
using FanScore.Application.DTOs.Player;

namespace FanScore.Application.DTOs.Admin
{
    public class AdminDashboardStatsDto
    {
        public int UserCount { get; set; }
        public int BannedUserCount { get; set; }
        public int ProductCount { get; set; }
        public int HalisahaCount { get; set; }
        public int PlayerCount { get; set; }
        public int RatingCount { get; set; }
    }

    public class AdminUserDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public DateTime? CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        public string? ProfilePic { get; set; }
        public bool IsBanned { get; set; }
        public string? BanReason { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class AdminBanUserDto
    {
        public string? BanReason { get; set; }
    }

    public class AdminChangeRoleDto
    {
        public string Role { get; set; } = "user";
    }

    public class AdminUpdateProductDto : ProductUpdateDto
    {
    }

    public class AdminUpdateHalisahaDto : TournamentUpdateDto
    {
    }

    public class AdminCreatePlayerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public int? TeamId { get; set; }
        public int? Age { get; set; }
        public string? Position { get; set; }
        public string? PpUrl { get; set; }
    }

    public class AdminUpdatePlayerDto : AdminCreatePlayerDto
    {
    }

    public class AdminPanelDto
    {
        public AdminDashboardStatsDto Stats { get; set; } = new();
        public List<AdminUserDto> Users { get; set; } = new();
        public List<ProductListDto> Products { get; set; } = new();
        public List<TournamentListDto> Halisahas { get; set; } = new();
        public List<PlayerDto> Players { get; set; } = new();
    }
}
