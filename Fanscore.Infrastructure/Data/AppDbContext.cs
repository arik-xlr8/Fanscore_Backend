using System;
using System.Collections.Generic;
using FanScore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FanScore.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Halisaha> Halisahas { get; set; }

    public virtual DbSet<Pic> Pics { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseMySql("server=localhost;port=3306;database=fanscore_db;user=root;password=Benkomando123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Halisaha>(entity =>
        {
            entity.HasKey(e => e.HaliSahaId).HasName("PRIMARY");

            entity
                .ToTable("halisaha")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.UserId, "idx_halisaha_user");

            entity.Property(e => e.HaliSahaId).HasColumnName("HaliSahaID");
            entity.Property(e => e.City).HasMaxLength(80);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Halisahas)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_halisaha_user");
        });

        modelBuilder.Entity<Pic>(entity =>
        {
            entity.HasKey(e => e.PicId).HasName("PRIMARY");

            entity
                .ToTable("pics")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.ProductId, "idx_pics_product");

            entity.Property(e => e.PicId).HasColumnName("PicID");
            entity.Property(e => e.PicUrl).HasMaxLength(512);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Product).WithMany(p => p.Pics)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_pics_product");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PRIMARY");

            entity
                .ToTable("player")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.TeamId, "idx_player_team");

            entity.HasIndex(e => new { e.Name, e.Surname, e.TeamId }, "uq_player_team_name").IsUnique();

            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_player_team");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity
                .ToTable("product")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.PicId, "idx_product_pic");

            entity.HasIndex(e => e.UserId, "idx_product_user");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ListedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.PicId).HasColumnName("PicID");
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Pic).WithMany(p => p.Products)
                .HasForeignKey(d => d.PicId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_product_pic");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_product_user");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PRIMARY");

            entity
                .ToTable("rating")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");


            entity.HasIndex(e => e.PlayerId, "idx_rating_player");

            entity.HasIndex(e => e.UserId, "idx_rating_user");


            entity.Property(e => e.RatingId).HasColumnName("RatingID");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PeriodType).HasMaxLength(20);
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.PlayerId).HasColumnName("PlayerID");
            entity.Property(e => e.RatingValue).HasPrecision(3, 1);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Player).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("fk_rating_player");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_rating_user");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PRIMARY");

            entity
                .ToTable("team")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.TeamName, "uq_team_name").IsUnique();

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TeamName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity
                .ToTable("user")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Email, "uq_user_email").IsUnique();

            entity.HasIndex(e => e.UserName, "uq_user_username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.BanReason)
                .HasMaxLength(255)
                .HasColumnName("banReason");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(512);
            entity.Property(e => e.IsBanned).HasColumnName("isBanned");
            entity.Property(e => e.IsVerified).HasColumnName("isVerified");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.NewPasswordTemp)
                .HasMaxLength(255)
                .HasColumnName("newPasswordTemp");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(512);
            entity.Property(e => e.PasswordResetTokenExpires).HasColumnType("datetime");
            entity.Property(e => e.ProfilePic).HasMaxLength(512);
            entity.Property(e => e.RefreshToken).HasMaxLength(512);
            entity.Property(e => e.RefreshTokenExpires).HasColumnType("datetime");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasDefaultValueSql("'user'");
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
