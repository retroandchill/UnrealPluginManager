using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Local.Migrations
{
    /// <inheritdoc />
    public partial class SeparatePluginDirectories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadedPlugins");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Plugins");

            migrationBuilder.AddColumn<string>(
                name: "VersionString",
                table: "PluginVersions",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PluginBinaries",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    EngineVersion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginBinaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginBinaries_PluginVersions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PluginVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PluginBinaries_EngineVersion_Platform",
                table: "PluginBinaries",
                columns: new[] { "EngineVersion", "Platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PluginBinaries_ParentId",
                table: "PluginBinaries",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginBinaries");

            migrationBuilder.DropColumn(
                name: "VersionString",
                table: "PluginVersions");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Plugins",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UploadedPlugins",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    EngineVersion = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedPlugins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedPlugins_PluginVersions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PluginVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadedPlugins_EngineVersion",
                table: "UploadedPlugins",
                column: "EngineVersion");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedPlugins_ParentId",
                table: "UploadedPlugins",
                column: "ParentId");
        }
    }
}
