using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public class GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : DbContext(options)
{
    private static readonly Guid GenreStrategyId = Guid.Parse("2b1d7a14-6f01-4f2d-b4b2-2a3461f9f701");
    private static readonly Guid GenreRtsId = Guid.Parse("9c3d67b9-5cc8-48cc-b4bb-9c2f30bf24aa");
    private static readonly Guid GenreTbsId = Guid.Parse("a0d0fe2a-b588-4c36-941b-3e14970b5237");
    private static readonly Guid GenreRpgId = Guid.Parse("e7c2e5a0-6b06-4785-9b65-2f57a3c0f1d2");
    private static readonly Guid GenreSportsId = Guid.Parse("c1bde7c2-1b0a-46ef-9a54-0c882c5c9c11");
    private static readonly Guid GenreRacesId = Guid.Parse("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c");
    private static readonly Guid GenreRallyId = Guid.Parse("9e9b6f40-7b4d-4cf9-93a9-bc0e2f594c85");
    private static readonly Guid GenreArcadeId = Guid.Parse("87c7e2d0-8f9c-4e52-946a-0d0b8f8af879");
    private static readonly Guid GenreFormulaId = Guid.Parse("4c1b90ac-5c2c-4a56-a357-236f8380b40b");
    private static readonly Guid GenreOffRoadId = Guid.Parse("b7412f02-90c1-4fbb-95b8-bc90d1b4f3d0");
    private static readonly Guid GenreActionId = Guid.Parse("e9f7c2a4-fb9e-4f1f-9d3f-6a4f3b14bd6c");
    private static readonly Guid GenreFpsId = Guid.Parse("b2f95f0b-bb3a-4d82-9f5a-4a31f2c2a8a4");
    private static readonly Guid GenreTpsId = Guid.Parse("9d47a4b2-6f1e-4f5a-8d8b-ef8228f52f11");
    private static readonly Guid GenreAdventureId = Guid.Parse("0b5c97c5-1f41-4cb1-b968-7e7f1a12f702");
    private static readonly Guid GenrePuzzleId = Guid.Parse("ea1a5e5a-8f3f-4a88-9806-7ff0a9e9f2cc");

    private static readonly Guid PlatformMobileId = Guid.Parse("4cc03e69-7a7b-4a08-bc79-7db7a4b7b08c");
    private static readonly Guid PlatformBrowserId = Guid.Parse("c0864f05-19d8-4d02-9c9b-2f25b2e1cb28");
    private static readonly Guid PlatformDesktopId = Guid.Parse("a51b02b3-2d1f-4b9b-8f38-8a1f7f0abde3");
    private static readonly Guid PlatformConsoleId = Guid.Parse("0e6c8b5f-0e69-4c1c-8d2b-5d1a4a7b2d4f");

    public DbSet<Game> Games => Set<Game>();

    public DbSet<Publisher> Publishers => Set<Publisher>();

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
            entity.HasOne(game => game.Publisher)
                .WithMany(publisher => publisher.Games)
                .HasForeignKey(game => game.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(publisher => publisher.Id);
            entity.Property(publisher => publisher.CompanyName).IsRequired();
            entity.HasIndex(publisher => publisher.CompanyName).IsUnique();
            entity.Property(publisher => publisher.HomePage).HasMaxLength(500);
            entity.Property(publisher => publisher.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(genre => genre.Id);
            entity.Property(genre => genre.Name).IsRequired();
            entity.HasIndex(genre => genre.Name).IsUnique();
            entity.HasOne(genre => genre.ParentGenre)
                .WithMany(genre => genre.SubGenres)
                .HasForeignKey(genre => genre.ParentGenreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasKey(platform => platform.Id);
            entity.Property(platform => platform.Type).IsRequired();
            entity.HasIndex(platform => platform.Type).IsUnique();
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(link => new { link.GameId, link.GenreId });
            entity.HasOne(link => link.Game)
                .WithMany(game => game.GameGenres)
                .HasForeignKey(link => link.GameId);
            entity.HasOne(link => link.Genre)
                .WithMany(genre => genre.GameGenres)
                .HasForeignKey(link => link.GenreId);
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

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = GenreStrategyId, Name = "Strategy", ParentGenreId = null },
            new Genre { Id = GenreRtsId, Name = "RTS", ParentGenreId = GenreStrategyId },
            new Genre { Id = GenreTbsId, Name = "TBS", ParentGenreId = GenreStrategyId },
            new Genre { Id = GenreRpgId, Name = "RPG", ParentGenreId = null },
            new Genre { Id = GenreSportsId, Name = "Sports", ParentGenreId = null },
            new Genre { Id = GenreRacesId, Name = "Races", ParentGenreId = null },
            new Genre { Id = GenreRallyId, Name = "Rally", ParentGenreId = GenreRacesId },
            new Genre { Id = GenreArcadeId, Name = "Arcade", ParentGenreId = GenreRacesId },
            new Genre { Id = GenreFormulaId, Name = "Formula", ParentGenreId = GenreRacesId },
            new Genre { Id = GenreOffRoadId, Name = "Off-road", ParentGenreId = GenreRacesId },
            new Genre { Id = GenreActionId, Name = "Action", ParentGenreId = null },
            new Genre { Id = GenreFpsId, Name = "FPS", ParentGenreId = GenreActionId },
            new Genre { Id = GenreTpsId, Name = "TPS", ParentGenreId = GenreActionId },
            new Genre { Id = GenreAdventureId, Name = "Adventure", ParentGenreId = null },
            new Genre { Id = GenrePuzzleId, Name = "Puzzle & Skill", ParentGenreId = null });

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = PlatformMobileId, Type = "Mobile" },
            new Platform { Id = PlatformBrowserId, Type = "Browser" },
            new Platform { Id = PlatformDesktopId, Type = "Desktop" },
            new Platform { Id = PlatformConsoleId, Type = "Console" });
    }
}
