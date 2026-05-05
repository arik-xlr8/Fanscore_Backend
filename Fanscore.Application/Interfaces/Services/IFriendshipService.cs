using Fanscore.Application.DTOs.Friendship;

namespace Fanscore.Application.Interfaces.Services
{
    public interface IFriendshipService
    {
        Task<List<FriendUserDto>> SearchUsersAsync(int currentUserId, string? searchTerm);
        Task<List<FriendUserDto>> GetFriendsAsync(int currentUserId);
        Task<List<FriendUserDto>> GetIncomingRequestsAsync(int currentUserId);
        Task<bool> AddFriendAsync(int currentUserId, int friendUserId);
        Task<bool> AcceptFriendRequestAsync(int currentUserId, int requesterUserId);
        Task<bool> RejectFriendRequestAsync(int currentUserId, int requesterUserId);
        Task<bool> RemoveFriendAsync(int currentUserId, int friendUserId);
        Task<FriendRatingHistoryDto> GetFriendRatingHistoryAsync(int currentUserId, int friendUserId);
    }
}
