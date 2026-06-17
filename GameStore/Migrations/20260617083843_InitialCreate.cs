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
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BannedUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPermanent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
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
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
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
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HomePage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitInStock = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "GameOrders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    ProductKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOrders", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_GameOrders_Games_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
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
                table: "Publishers",
                columns: new[] { "Id", "CompanyName", "Description", "HomePage" },
                values: new object[,]
                {
                    { new Guid("9c8b7a6f-5e4d-3c2b-1a09-f8e7d6c5b4a3"), "Ubisoft", null, null },
                    { new Guid("a1b2c3d4-e5f6-4a8b-9c0d-1e2f3a4b5c6d"), "Electronic Arts", null, null },
                    { new Guid("f5b8c1a0-9f3c-4e2d-b5a6-3c7d8e9f0a1b"), "Activision", null, null }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Description", "Discount", "Key", "Name", "Price", "PublisherId", "UnitInStock" },
                values: new object[,]
                {
                    { new Guid("2f4e6a8c-9b1d-4c3f-7e9a-1b5d8c2f4a6e"), "Become a legendary Viking warrior.", 10, "ac-valhalla", "Assassin's Creed Valhalla", 49.99m, new Guid("9c8b7a6f-5e4d-3c2b-1a09-f8e7d6c5b4a3"), 75 },
                    { new Guid("5a9f2b8d-3c7e-4a1f-6d9c-2e5b7a1f4c8d"), "Play with life! Control the Sims' destiny and explore the world.", 0, "sims-4", "The Sims 4", 39.99m, new Guid("a1b2c3d4-e5f6-4a8b-9c0d-1e2f3a4b5c6d"), 150 },
                    { new Guid("7d3c8e2f-4b6a-4d9f-8e2c-5a7b9c1d3e4f"), "Experience an intimate, grounded, cooperative, and playable Campaign.", 0, "cod-mw", "Call of Duty: Modern Warfare", 59.99m, new Guid("f5b8c1a0-9f3c-4e2d-b5a6-3c7d8e9f0a1b"), 100 }
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
                name: "IX_Comments_GameId",
                table: "Comments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGenres_GenreId",
                table: "GameGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOrders_ProductId",
                table: "GameOrders",
                column: "ProductId");

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
                name: "IX_Games_PublisherId",
                table: "Games",
                column: "PublisherId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_CompanyName",
                table: "Publishers",
                column: "CompanyName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "GameGenres");

            migrationBuilder.DropTable(
                name: "GameOrders");

            migrationBuilder.DropTable(
                name: "GamePlatforms");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
