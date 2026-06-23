using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Migrations
{
    /// <inheritdoc />
    public partial class AddGameViewCountAndCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("2f4e6a8c-9b1d-4c3f-7e9a-1b5d8c2f4a6e"),
                columns: new[] { "CreatedAt", "ViewCount" },
                values: new object[] { new DateTime(2026, 6, 23, 12, 12, 28, 509, DateTimeKind.Utc).AddTicks(1973), 0 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("5a9f2b8d-3c7e-4a1f-6d9c-2e5b7a1f4c8d"),
                columns: new[] { "CreatedAt", "ViewCount" },
                values: new object[] { new DateTime(2026, 6, 23, 12, 12, 28, 509, DateTimeKind.Utc).AddTicks(1977), 0 });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("7d3c8e2f-4b6a-4d9f-8e2c-5a7b9c1d3e4f"),
                columns: new[] { "CreatedAt", "ViewCount" },
                values: new object[] { new DateTime(2026, 6, 23, 12, 12, 28, 509, DateTimeKind.Utc).AddTicks(1961), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Games");
        }
    }
}
