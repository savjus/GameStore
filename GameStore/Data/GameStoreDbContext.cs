using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public class GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Platform> Platforms => Set<Platform>();

    public DbSet<GameGenre> GameGenres => Set<GameGenre>();

    public DbSet<GamePlatform> GamePlatforms => Set<GamePlatform>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(game => game.Id);
            entity.Property(game => game.Name).IsRequired();
            entity.Property(game => game.Key).IsRequired();
            entity.HasIndex(game => game.Key).IsUnique();
            entity.Property(game => game.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(genre => genre.Id);
            entity.Property(genre => genre.Name).IsRequired();
            entity.HasIndex(genre => genre.Name).IsUnique();
            entity.HasOne(g => g.ParentGenre)
                  .WithMany(g => g.SubGenres)
                  .HasForeignKey(g => g.ParentGenreId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(platform => platform.Id);
            entity.Property(platform => platform.Type).IsRequired();
            entity.HasIndex(platform => platform.Type).IsUnique();
        });

        modelBuilder.Entity<GamePlatform>(entity =>
        {
            entity.HasKey(link => new { link.GameId, link.PlatformId });
            entity.HasOne(link => link.Game)
                .WithMany(game => game.GamePlatforms)
                .HasForeignKey(link => link.GameId);
            entity.HasOne(link => link.Platform)
                .WithMany(platform => platform.GamePlatforms)
                .HasForeignKey(link => link.PlatformId);
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(link => new { link.GameId, link.GenreId });
            entity.HasOne(link => link.Game)
                .WithMany(game => game.GameGenres)
                .HasForeignKey(link => link.GameId);
            entity.HasOne(link => link.Genre)
                .WithMany(genre => genre.GameGenres)
                .HasForeignKey(link => link.Genre);
        });
    }
}