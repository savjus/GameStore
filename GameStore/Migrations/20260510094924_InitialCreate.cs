using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameStore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentGenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genres_Genres_ParentGenreId",
                        column: x => x.ParentGenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameGenres",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGenres", x => new { x.GameId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_GameGenres_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePlatforms",
                columns: table => new
                {
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlatforms", x => new { x.GameId, x.PlatformId });
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentGenreId" },
                values: new object[,]
                {
                    { new Guid("0b5c97c5-1f41-4cb1-b968-7e7f1a12f702"), "Adventure", null },
                    { new Guid("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c"), "Races", null },
                    { new Guid("2b1d7a14-6f01-4f2d-b4b2-2a3461f9f701"), "Strategy", null },
                    { new Guid("c1bde7c2-1b0a-46ef-9a54-0c882c5c9c11"), "Sports", null },
                    { new Guid("e7c2e5a0-6b06-4785-9b65-2f57a3c0f1d2"), "RPG", null },
                    { new Guid("e9f7c2a4-fb9e-4f1f-9d3f-6a4f3b14bd6c"), "Action", null },
                    { new Guid("ea1a5e5a-8f3f-4a88-9806-7ff0a9e9f2cc"), "Puzzle & Skill", null }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { new Guid("0e6c8b5f-0e69-4c1c-8d2b-5d1a4a7b2d4f"), "Console" },
                    { new Guid("4cc03e69-7a7b-4a08-bc79-7db7a4b7b08c"), "Mobile" },
                    { new Guid("a51b02b3-2d1f-4b9b-8f38-8a1f7f0abde3"), "Desktop" },
                    { new Guid("c0864f05-19d8-4d02-9c9b-2f25b2e1cb28"), "Browser" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name", "ParentGenreId" },
                values: new object[,]
                {
                    { new Guid("4c1b90ac-5c2c-4a56-a357-236f8380b40b"), "Formula", new Guid("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c") },
                    { new Guid("87c7e2d0-8f9c-4e52-946a-0d0b8f8af879"), "Arcade", new Guid("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c") },
                    { new Guid("9c3d67b9-5cc8-48cc-b4bb-9c2f30bf24aa"), "RTS", new Guid("2b1d7a14-6f01-4f2d-b4b2-2a3461f9f701") },
                    { new Guid("9d47a4b2-6f1e-4f5a-8d8b-ef8228f52f11"), "TPS", new Guid("e9f7c2a4-fb9e-4f1f-9d3f-6a4f3b14bd6c") },
                    { new Guid("9e9b6f40-7b4d-4cf9-93a9-bc0e2f594c85"), "Rally", new Guid("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c") },
                    { new Guid("a0d0fe2a-b588-4c36-941b-3e14970b5237"), "TBS", new Guid("2b1d7a14-6f01-4f2d-b4b2-2a3461f9f701") },
                    { new Guid("b2f95f0b-bb3a-4d82-9f5a-4a31f2c2a8a4"), "FPS", new Guid("e9f7c2a4-fb9e-4f1f-9d3f-6a4f3b14bd6c") },
                    { new Guid("b7412f02-90c1-4fbb-95b8-bc90d1b4f3d0"), "Off-road", new Guid("1e0e2f11-2b2b-4f5f-8b0e-50ed3d70ed1c") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGenres_GenreId",
                table: "GameGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlatforms_PlatformId",
                table: "GamePlatforms",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Key",
                table: "Games",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ParentGenreId",
                table: "Genres",
                column: "ParentGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Type",
                table: "Platforms",
                column: "Type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameGenres");

            migrationBuilder.DropTable(
                name: "GamePlatforms");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Platforms");
        }
    }
}
