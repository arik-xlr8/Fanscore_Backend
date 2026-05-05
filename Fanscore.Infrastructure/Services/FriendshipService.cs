using Fanscore.Application.DTOs.Friendship;
using Fanscore.Application.DTOs.User;
using Fanscore.Application.Interfaces.Services;
using Fanscore.Domain.Entities;
using FanScore.Domain.Entities;
using FanScore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fanscore.Infrastructure.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly AppDbContext _context;

        public FriendshipService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FriendUserDto>> SearchUsersAsync(int currentUserId, string? searchTerm)
        {
            var term = searchTerm?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(term))
                return new List<FriendUserDto>();

            var relationships = await GetRelationshipMapAsync(currentUserId);

            return await _context.Users
                .Where(u => u.UserId != currentUserId && u.IsBanned != true)
                .Where(u =>
                    u.Email.ToLower().Contains(term) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                    (u.Name != null && u.Name.ToLower().Contains(term)) ||
                    (u.Surname != null && u.Surname.ToLower().Contains(term)))
                .OrderBy(u => u.UserName ?? u.Email)
                .Take(10)
                .Select(u => new FriendUserDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Name = u.Name,
                    Surname = u.Surname,
                    Email = u.Email,
                    ProfilePic = u.ProfilePic,
                    FriendSince = relationships.ContainsKey(u.UserId) && relationships[u.UserId].Status == "Accepted"
                        ? relationships[u.UserId].CreatedAt
                        : null,
                    Status = relationships.ContainsKey(u.UserId) ? relationships[u.UserId].Status : null,
                    IsIncomingRequest = relationships.ContainsKey(u.UserId) &&
                        relationships[u.UserId].Status == "Pending" &&
                        relationships[u.UserId].RequestedByUserId == u.UserId,
                    IsOutgoingRequest = relationships.ContainsKey(u.UserId) &&
                        relationships[u.UserId].Status == "Pending" &&
                        relationships[u.UserId].RequestedByUserId == currentUserId
                })
                .ToListAsync();
        }

        public async Task<List<FriendUserDto>> GetFriendsAsync(int currentUserId)
        {
            return await _context.Friendships
                .Where(f => f.Status == "Accepted" && (f.UserId == currentUserId || f.FriendUserId == currentUserId))
                .Select(f => new
                {
                    Friendship = f,
                    Friend = f.UserId == currentUserId ? f.FriendUser : f.User
                })
                .OrderByDescending(x => x.Friendship.CreatedAt)
                .Select(x => new FriendUserDto
                {
                    UserId = x.Friend.UserId,
                    UserName = x.Friend.UserName,
                    Name = x.Friend.Name,
                    Surname = x.Friend.Surname,
                    Email = x.Friend.Email,
                    ProfilePic = x.Friend.ProfilePic,
                    FriendSince = x.Friendship.RespondedAt ?? x.Friendship.CreatedAt,
                    Status = x.Friendship.Status
                })
                .ToListAsync();
        }

        public async Task<List<FriendUserDto>> GetIncomingRequestsAsync(int currentUserId)
        {
            return await _context.Friendships
                .Where(f => f.Status == "Pending" && f.RequestedByUserId != currentUserId)
                .Where(f => f.UserId == currentUserId || f.FriendUserId == currentUserId)
                .Select(f => new
                {
                    Friendship = f,
                    Requester = f.RequestedByUser
                })
                .OrderByDescending(x => x.Friendship.CreatedAt)
                .Select(x => new FriendUserDto
                {
                    UserId = x.Requester.UserId,
                    UserName = x.Requester.UserName,
                    Name = x.Requester.Name,
                    Surname = x.Requester.Surname,
                    Email = x.Requester.Email,
                    ProfilePic = x.Requester.ProfilePic,
                    FriendSince = x.Friendship.CreatedAt,
                    Status = x.Friendship.Status,
                    IsIncomingRequest = true
                })
                .ToListAsync();
        }

        public async Task<bool> AddFriendAsync(int currentUserId, int friendUserId)
        {
            if (currentUserId == friendUserId)
                throw new InvalidOperationException("Kendinizi arkadas ekleyemezsiniz.");

            var friendExists = await _context.Users.AnyAsync(u => u.UserId == friendUserId && u.IsBanned != true);
            if (!friendExists)
                return false;

            var (firstUserId, secondUserId) = NormalizePair(currentUserId, friendUserId);

            var exists = await _context.Friendships.AnyAsync(f =>
                f.UserId == firstUserId && f.FriendUserId == secondUserId);

            if (exists)
                return true;

            _context.Friendships.Add(new Friendship
            {
                UserId = firstUserId,
                FriendUserId = secondUserId,
                RequestedByUserId = currentUserId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AcceptFriendRequestAsync(int currentUserId, int requesterUserId)
        {
            var (firstUserId, secondUserId) = NormalizePair(currentUserId, requesterUserId);

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.UserId == firstUserId &&
                f.FriendUserId == secondUserId &&
                f.Status == "Pending" &&
                f.RequestedByUserId == requesterUserId);

            if (friendship == null)
                return false;

            friendship.Status = "Accepted";
            friendship.RespondedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectFriendRequestAsync(int currentUserId, int requesterUserId)
        {
            var (firstUserId, secondUserId) = NormalizePair(currentUserId, requesterUserId);

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.UserId == firstUserId &&
                f.FriendUserId == secondUserId &&
                f.Status == "Pending" &&
                f.RequestedByUserId == requesterUserId);

            if (friendship == null)
                return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFriendAsync(int currentUserId, int friendUserId)
        {
            var (firstUserId, secondUserId) = NormalizePair(currentUserId, friendUserId);

            var friendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                f.UserId == firstUserId && f.FriendUserId == secondUserId);

            if (friendship == null)
                return false;

            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<FriendRatingHistoryDto> GetFriendRatingHistoryAsync(int currentUserId, int friendUserId)
        {
            var isFriend = await AreFriendsAsync(currentUserId, friendUserId);
            if (!isFriend)
                throw new UnauthorizedAccessException("Bu kullanicinin rating gecmisini gorebilmek icin arkadas olmalisiniz.");

            var friend = await _context.Users.FirstOrDefaultAsync(u => u.UserId == friendUserId);
            if (friend == null)
                throw new KeyNotFoundException("Arkadas bulunamadi.");

            var ratings = await (
                from r in _context.Ratings
                join p in _context.Players on r.PlayerId equals p.PlayerId
                where r.UserId == friendUserId
                orderby r.CreatedAt descending
                select new MyRecentRatingDto
                {
                    RatingId = r.RatingId,
                    PlayerId = p.PlayerId,
                    PlayerName = ((p.Name ?? "") + " " + (p.Surname ?? "")).Trim(),
                    PlayerPhoto = p.PpUrl,
                    RatingValue = r.RatingValue,
                    Comment = r.Comment,
                    PeriodType = r.PeriodType,
                    CreatedAt = r.CreatedAt
                }
            ).Take(30).ToListAsync();

            return new FriendRatingHistoryDto
            {
                Friend = new FriendUserDto
                {
                    UserId = friend.UserId,
                    UserName = friend.UserName,
                    Name = friend.Name,
                    Surname = friend.Surname,
                    Email = friend.Email,
                    ProfilePic = friend.ProfilePic
                },
                Ratings = ratings
            };
        }

        private async Task<HashSet<int>> GetFriendIdsAsync(int currentUserId)
        {
            var ids = await _context.Friendships
                .Where(f => f.UserId == currentUserId || f.FriendUserId == currentUserId)
                .Select(f => f.UserId == currentUserId ? f.FriendUserId : f.UserId)
                .ToListAsync();

            return ids.ToHashSet();
        }

        private async Task<bool> AreFriendsAsync(int currentUserId, int friendUserId)
        {
            var (firstUserId, secondUserId) = NormalizePair(currentUserId, friendUserId);

            return await _context.Friendships.AnyAsync(f =>
                f.UserId == firstUserId && f.FriendUserId == secondUserId && f.Status == "Accepted");
        }

        private async Task<Dictionary<int, Friendship>> GetRelationshipMapAsync(int currentUserId)
        {
            var relationships = await _context.Friendships
                .Where(f => f.UserId == currentUserId || f.FriendUserId == currentUserId)
                .ToListAsync();

            return relationships.ToDictionary(
                f => f.UserId == currentUserId ? f.FriendUserId : f.UserId,
                f => f);
        }

        private static (int FirstUserId, int SecondUserId) NormalizePair(int userId, int friendUserId)
        {
            return userId < friendUserId
                ? (userId, friendUserId)
                : (friendUserId, userId);
        }
    }
}
