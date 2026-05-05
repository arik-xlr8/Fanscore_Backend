using Fanscore.Application.DTOs.User;

namespace Fanscore.Application.DTOs.Friendship
{
    public class FriendUserDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? ProfilePic { get; set; }
        public DateTime? FriendSince { get; set; }
        public string? Status { get; set; }
        public bool IsIncomingRequest { get; set; }
        public bool IsOutgoingRequest { get; set; }
    }

    public class FriendRatingHistoryDto
    {
        public FriendUserDto Friend { get; set; } = new();
        public List<MyRecentRatingDto> Ratings { get; set; } = new();
    }
}
