using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrowsmartAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastFertilizedAt",
                table: "Plants",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWateredAt",
                table: "Plants",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastFertilizedAt",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "LastWateredAt",
                table: "Plants");
        }
    }
}
