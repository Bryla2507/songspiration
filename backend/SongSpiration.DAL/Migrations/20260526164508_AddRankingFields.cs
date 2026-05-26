using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongSpiration.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditorChoice",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DownloadsCount",
                table: "Pins",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEditorChoice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DownloadsCount",
                table: "Pins");
        }
    }
}
