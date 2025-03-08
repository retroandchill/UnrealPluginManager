using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PluginBinaries_EngineVersion_Platform",
                table: "PluginBinaries");

            migrationBuilder.CreateIndex(
                name: "IX_PluginBinaries_ParentId_EngineVersion_Platform",
                table: "PluginBinaries",
                columns: new[] { "ParentId", "EngineVersion", "Platform" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PluginBinaries_ParentId_EngineVersion_Platform",
                table: "PluginBinaries");

            migrationBuilder.CreateIndex(
                name: "IX_PluginBinaries_EngineVersion_Platform",
                table: "PluginBinaries",
                columns: new[] { "EngineVersion", "Platform" },
                unique: true);
        }
    }
}
