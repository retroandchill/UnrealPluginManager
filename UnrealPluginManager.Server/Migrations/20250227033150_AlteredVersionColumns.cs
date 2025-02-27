using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class AlteredVersionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PluginVersions_Version",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PluginVersions");

            migrationBuilder.AddColumn<int>(
                name: "Major",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "PluginVersions",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Minor",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Patch",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Prerelease",
                table: "PluginVersions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrereleaseNumber",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Major",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Minor",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Patch",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Prerelease",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "PrereleaseNumber",
                table: "PluginVersions");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "PluginVersions",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersions_Version",
                table: "PluginVersions",
                column: "Version",
                unique: true);
        }
    }
}
