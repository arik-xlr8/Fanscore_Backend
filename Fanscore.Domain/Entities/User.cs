using System;
using System.Collections.Generic;
using Fanscore.Domain.Entities;

namespace FanScore.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpires { get; set; }

    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpires { get; set; }
    public DateTime? LastVerificationEmailSentAt { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? IsVerified { get; set; }

    public string? ProfilePic { get; set; }

    public bool? IsBanned { get; set; }

    public string? BanReason { get; set; }

    public string? NewPasswordTemp { get; set; }

    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpires { get; set; }

    public virtual ICollection<Halisaha> Halisahas { get; set; } = new List<Halisaha>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<RatingReaction> RatingReactions { get; set; } = new List<RatingReaction>();

}