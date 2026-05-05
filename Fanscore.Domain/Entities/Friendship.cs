using FanScore.Domain.Entities;

namespace Fanscore.Domain.Entities;

public partial class Friendship
{
    public int FriendshipId { get; set; }

    public int UserId { get; set; }

    public int FriendUserId { get; set; }

    public int RequestedByUserId { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }

    public DateTime? RespondedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual User FriendUser { get; set; } = null!;

    public virtual User RequestedByUser { get; set; } = null!;
}
