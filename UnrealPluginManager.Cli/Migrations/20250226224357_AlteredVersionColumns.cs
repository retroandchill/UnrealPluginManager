using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Cli.Migrations
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

            migrationBuilder.AddColumn<long>(
                name: "Version_Major",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Version_Metadata",
                table: "PluginVersions",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Version_Minor",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Version_Patch",
                table: "PluginVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Version_Prerelease",
                table: "PluginVersions",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersions_Version_Major_Version_Minor_Version_Patch_Version_Prerelease_Version_Metadata",
                table: "PluginVersions",
                columns: new[] { "Version_Major", "Version_Minor", "Version_Patch", "Version_Prerelease", "Version_Metadata" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PluginVersions_Version_Major_Version_Minor_Version_Patch_Version_Prerelease_Version_Metadata",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Version_Major",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Version_Metadata",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Version_Minor",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Version_Patch",
                table: "PluginVersions");

            migrationBuilder.DropColumn(
                name: "Version_Prerelease",
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
