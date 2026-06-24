using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

public static class DatabaseSeeder
{
    private static readonly Guid PublisherBethesdaId = Guid.Parse("c2d3e4f5-a6b7-4c8d-9e0f-2a3b4c5d6e7f");
    private static readonly Guid Publisher2KGamesId = Guid.Parse("b1c2d3e4-f5a6-4b8c-9d0e-1f2a3b4c5d6e");
    private static readonly Guid PublisherNintendoId = Guid.Parse("d3e4f5a6-b7c8-4d9e-0f1a-3b4c5d6e7f8a");

    private static readonly Guid GameDestiny2Id = Guid.Parse("a1b1c1d1-e1f1-4a1b-8c1d-1e2f3a4b5c6d");
    private static readonly Guid GameFifa23Id = Guid.Parse("a2b2c2d2-e2f2-4a2b-8c2d-2e3f4a5b6c7d");
    private static readonly Guid GameBattlefield2042Id = Guid.Parse("a3b3c3d3-e3f3-4a3b-8c3d-3e4f5a6b7c8d");
    private static readonly Guid GameFarCry6Id = Guid.Parse("a4b4c4d4-e4f4-4a4b-8c4d-4e5f6a7b8c9d");
    private static readonly Guid GameR6SiegeId = Guid.Parse("a5b5c5d5-e5f5-4a5b-8c5d-5e6f7a8b9c0d");
    private static readonly Guid GameSkyrimId = Guid.Parse("a6b6c6d6-e6f6-4a6b-8c6d-6e7f8a9b0c1d");
    private static readonly Guid GameFallout4Id = Guid.Parse("a7b7c7d7-e7f7-4a7b-8c7d-7e8f9a0b1c2d");
    private static readonly Guid GameBioshockId = Guid.Parse("a8b8c8d8-e8f8-4a8b-8c8d-8e9f0a1b2c3d");
    private static readonly Guid GameBorderlands3Id = Guid.Parse("a9b9c9d9-e9f9-4a9b-8c9d-9e0f1a2b3c4d");
    private static readonly Guid GameMarioKartId = Guid.Parse("b0c0d0e0-f0a0-4b0c-9d0e-0f1a2b3c4d5e");

    private static readonly Guid GameCallOfDutyId = Guid.Parse("7d3c8e2f-4b6a-4d9f-8e2c-5a7b9c1d3e4f");
    private static readonly Guid GameAssassinsCreedId = Guid.Parse("2f4e6a8c-9b1d-4c3f-7e9a-1b5d8c2f4a6e");
    private static readonly Guid GameTheSimsId = Guid.Parse("5a9f2b8d-3c7e-4a1f-6d9c-2e5b7a1f4c8d");

    private static readonly Guid GenreFpsId = Guid.Parse("b2f95f0b-bb3a-4d82-9f5a-4a31f2c2a8a4");
    private static readonly Guid GenreTpsId = Guid.Parse("9d47a4b2-6f1e-4f5a-8d8b-ef8228f52f11");
    private static readonly Guid GenreActionId = Guid.Parse("e9f7c2a4-fb9e-4f1f-9d3f-6a4f3b14bd6c");
    private static readonly Guid GenreAdventureId = Guid.Parse("0b5c97c5-1f41-4cb1-b968-7e7f1a12f702");
    private static readonly Guid GenreRpgId = Guid.Parse("e7c2e5a0-6b06-4785-9b65-2f57a3c0f1d2");
    private static readonly Guid GenreSportsId = Guid.Parse("c1bde7c2-1b0a-46ef-9a54-0c882c5c9c11");
    private static readonly Guid GenreRacesId = Guid.Parse("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c");
    private static readonly Guid GenreArcadeId = Guid.Parse("87c7e2d0-8f9c-4e52-946a-0d0b8f8af879");
    private static readonly Guid GenrePuzzleId = Guid.Parse("ea1a5e5a-8f3f-4a88-9806-7ff0a9e9f2cc");

    private static readonly Guid PlatformDesktopId = Guid.Parse("a51b02b3-2d1f-4b9b-8f38-8a1f7f0abde3");
    private static readonly Guid PlatformConsoleId = Guid.Parse("0e6c8b5f-0e69-4c1c-8d2b-5d1a4a7b2d4f");
    private static readonly Guid PlatformMobileId = Guid.Parse("4cc03e69-7a7b-4a08-bc79-7db7a4b7b08c");

    private static readonly Guid PublisherActivisionId = Guid.Parse("f5b8c1a0-9f3c-4e2d-b5a6-3c7d8e9f0a1b");
    private static readonly Guid PublisherEAId = Guid.Parse("a1b2c3d4-e5f6-4a8b-9c0d-1e2f3a4b5c6d");
    private static readonly Guid PublisherUbisoftId = Guid.Parse("9c8b7a6f-5e4d-3c2b-1a09-f8e7d6c5b4a3");

    public static async Task SeedAsync(GameStoreDbContext db)
    {
        await SeedPublishersAsync(db);
        await SeedGamesAsync(db);
        await SeedGameGenresAsync(db);
        await SeedGamePlatformsAsync(db);
        await db.SaveChangesAsync();
    }

    private static async Task SeedPublishersAsync(GameStoreDbContext db)
    {
        var existingIds = await db.Publishers.Select(p => p.Id).ToListAsync();

        var publishers = new List<Publisher>
        {
            new() { Id = PublisherBethesdaId, CompanyName = "Bethesda", HomePage = "https://bethesda.net", Description = "Creators of the Elder Scrolls and Fallout series." },
            new() { Id = Publisher2KGamesId, CompanyName = "2K Games", HomePage = "https://2k.com", Description = "Publisher of BioShock, Borderlands, and NBA 2K." },
            new() { Id = PublisherNintendoId, CompanyName = "Nintendo", HomePage = "https://nintendo.com", Description = "Creator of Mario, Zelda, and Pokemon." },
        };

        foreach (var publisher in publishers.Where(p => !existingIds.Contains(p.Id)))
        {
            db.Publishers.Add(publisher);
        }
    }

    private static async Task SeedGamesAsync(GameStoreDbContext db)
    {
        var existingIds = await db.Games.Select(g => g.Id).ToListAsync();

        var games = new List<Game>
        {
            new()
            {
                Id = GameDestiny2Id,
                Name = "Destiny 2",
                Key = "destiny-2",
                Description = "A free-to-play online-only multiplayer first-person shooter.",
                Price = 0M,
                Discount = 0,
                UnitInStock = 999,
                PublisherId = PublisherActivisionId,
                ViewCount = 0,
                CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameFifa23Id,
                Name = "EA SPORTS FC 24",
                Key = "ea-sports-fc-24",
                Description = "The world's most popular football simulation game.",
                Price = 59.99M,
                Discount = 15,
                UnitInStock = 200,
                PublisherId = PublisherEAId,
                ViewCount = 0,
                CreatedAt = new DateTime(2025, 9, 29, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameBattlefield2042Id,
                Name = "Battlefield 2042",
                Key = "battlefield-2042",
                Description = "Experience the ultimate battlefield with all-out warfare.",
                Price = 19.99M,
                Discount = 50,
                UnitInStock = 120,
                PublisherId = PublisherEAId,
                ViewCount = 0,
                CreatedAt = new DateTime(2025, 11, 19, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameFarCry6Id,
                Name = "Far Cry 6",
                Key = "far-cry-6",
                Description = "Welcome to Yara, a tropical paradise frozen in time.",
                Price = 29.99M,
                Discount = 20,
                UnitInStock = 90,
                PublisherId = PublisherUbisoftId,
                ViewCount = 0,
                CreatedAt = new DateTime(2025, 10, 7, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameR6SiegeId,
                Name = "Rainbow Six Siege",
                Key = "r6-siege",
                Description = "Master the art of destruction in a team-based tactical shooter.",
                Price = 14.99M,
                Discount = 0,
                UnitInStock = 500,
                PublisherId = PublisherUbisoftId,
                ViewCount = 0,
                CreatedAt = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameSkyrimId,
                Name = "The Elder Scrolls V: Skyrim",
                Key = "skyrim",
                Description = "Live another life in another world.",
                Price = 39.99M,
                Discount = 0,
                UnitInStock = 300,
                PublisherId = PublisherBethesdaId,
                ViewCount = 0,
                CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameFallout4Id,
                Name = "Fallout 4",
                Key = "fallout-4",
                Description = "You're the sole survivor of Vault 111, emerging from the 21st century.",
                Price = 19.99M,
                Discount = 25,
                UnitInStock = 250,
                PublisherId = PublisherBethesdaId,
                ViewCount = 0,
                CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameBioshockId,
                Name = "BioShock Infinite",
                Key = "bioshock-infinite",
                Description = "Indebted to the wrong people, Booker DeWitt must retrieve a girl named Elizabeth from the sky-city of Columbia.",
                Price = 14.99M,
                Discount = 10,
                UnitInStock = 180,
                PublisherId = Publisher2KGamesId,
                ViewCount = 0,
                CreatedAt = new DateTime(2024, 8, 20, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameBorderlands3Id,
                Name = "Borderlands 3",
                Key = "borderlands-3",
                Description = "Shoot and loot your way through a mayhem-fueled adventure.",
                Price = 39.99M,
                Discount = 30,
                UnitInStock = 140,
                PublisherId = Publisher2KGamesId,
                ViewCount = 0,
                CreatedAt = new DateTime(2025, 2, 14, 0, 0, 0, DateTimeKind.Utc),
            },
            new()
            {
                Id = GameMarioKartId,
                Name = "Mario Kart 8 Deluxe",
                Key = "mario-kart-8",
                Description = "Race with your friends and family in the definitive version of Mario Kart 8.",
                Price = 59.99M,
                Discount = 0,
                UnitInStock = 350,
                PublisherId = PublisherNintendoId,
                ViewCount = 0,
                CreatedAt = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
            },
        };

        foreach (var game in games.Where(g => !existingIds.Contains(g.Id)))
        {
            db.Games.Add(game);
        }
    }

    private static async Task SeedGameGenresAsync(GameStoreDbContext db)
    {
        var existing = await db.GameGenres.Select(gg => new { gg.GameId, gg.GenreId }).ToListAsync();

        var links = new List<(Guid GameId, Guid GenreId)>
        {
            (GameCallOfDutyId, GenreFpsId),
            (GameCallOfDutyId, GenreActionId),
            (GameAssassinsCreedId, GenreTpsId),
            (GameAssassinsCreedId, GenreActionId),
            (GameAssassinsCreedId, GenreAdventureId),
            (GameAssassinsCreedId, GenreRpgId),
            (GameTheSimsId, GenreAdventureId),
            (GameTheSimsId, GenrePuzzleId),
            (GameDestiny2Id, GenreFpsId),
            (GameDestiny2Id, GenreActionId),
            (GameFifa23Id, GenreSportsId),
            (GameBattlefield2042Id, GenreFpsId),
            (GameBattlefield2042Id, GenreActionId),
            (GameFarCry6Id, GenreTpsId),
            (GameFarCry6Id, GenreActionId),
            (GameFarCry6Id, GenreAdventureId),
            (GameR6SiegeId, GenreFpsId),
            (GameR6SiegeId, GenreActionId),
            (GameSkyrimId, GenreRpgId),
            (GameSkyrimId, GenreAdventureId),
            (GameFallout4Id, GenreRpgId),
            (GameFallout4Id, GenreActionId),
            (GameBioshockId, GenreFpsId),
            (GameBioshockId, GenreAdventureId),
            (GameBorderlands3Id, GenreFpsId),
            (GameBorderlands3Id, GenreRpgId),
            (GameBorderlands3Id, GenreActionId),
            (GameMarioKartId, GenreRacesId),
            (GameMarioKartId, GenreArcadeId),
        };

        foreach (var (gameId, genreId) in links)
        {
            if (!existing.Any(e => e.GameId == gameId && e.GenreId == genreId))
            {
                db.GameGenres.Add(new GameGenre { GameId = gameId, GenreId = genreId });
            }
        }
    }

    private static async Task SeedGamePlatformsAsync(GameStoreDbContext db)
    {
        var existing = await db.GamePlatforms.Select(gp => new { gp.GameId, gp.PlatformId }).ToListAsync();

        var links = new List<(Guid GameId, Guid PlatformId)>
        {
            (GameCallOfDutyId, PlatformDesktopId),
            (GameCallOfDutyId, PlatformConsoleId),
            (GameAssassinsCreedId, PlatformDesktopId),
            (GameAssassinsCreedId, PlatformConsoleId),
            (GameTheSimsId, PlatformDesktopId),
            (GameTheSimsId, PlatformMobileId),
            (GameDestiny2Id, PlatformDesktopId),
            (GameDestiny2Id, PlatformConsoleId),
            (GameFifa23Id, PlatformDesktopId),
            (GameFifa23Id, PlatformConsoleId),
            (GameFifa23Id, PlatformMobileId),
            (GameBattlefield2042Id, PlatformDesktopId),
            (GameBattlefield2042Id, PlatformConsoleId),
            (GameFarCry6Id, PlatformDesktopId),
            (GameFarCry6Id, PlatformConsoleId),
            (GameR6SiegeId, PlatformDesktopId),
            (GameR6SiegeId, PlatformConsoleId),
            (GameR6SiegeId, PlatformMobileId),
            (GameSkyrimId, PlatformDesktopId),
            (GameSkyrimId, PlatformConsoleId),
            (GameFallout4Id, PlatformDesktopId),
            (GameFallout4Id, PlatformConsoleId),
            (GameBioshockId, PlatformDesktopId),
            (GameBioshockId, PlatformConsoleId),
            (GameBorderlands3Id, PlatformDesktopId),
            (GameBorderlands3Id, PlatformConsoleId),
            (GameMarioKartId, PlatformConsoleId),
        };

        foreach (var (gameId, platformId) in links)
        {
            if (!existing.Any(e => e.GameId == gameId && e.PlatformId == platformId))
            {
                db.GamePlatforms.Add(new GamePlatform { GameId = gameId, PlatformId = platformId });
            }
        }
    }
}
