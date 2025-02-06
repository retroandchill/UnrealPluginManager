using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class EngineVersionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EngineVersion",
                table: "UploadedPlugins",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedPlugins_EngineVersion",
                table: "UploadedPlugins",
                column: "EngineVersion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UploadedPlugins_EngineVersion",
                table: "UploadedPlugins");

            migrationBuilder.DropColumn(
                name: "EngineVersion",
                table: "UploadedPlugins");
        }
    }
}
