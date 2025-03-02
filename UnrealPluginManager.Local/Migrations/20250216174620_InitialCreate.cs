using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Local.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FriendlyName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AuthorWebsite = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PluginVersions",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginVersions_Plugins_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dependency",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    PluginName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PluginVersion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Optional = table.Column<bool>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dependency_PluginVersions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PluginVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadedPlugins",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    EngineVersion = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "IX_Dependency_ParentId",
                table: "Dependency",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_Name",
                table: "Plugins",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersions_ParentId",
                table: "PluginVersions",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PluginVersions_Version",
                table: "PluginVersions",
                column: "Version",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedPlugins_EngineVersion",
                table: "UploadedPlugins",
                column: "EngineVersion");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedPlugins_ParentId",
                table: "UploadedPlugins",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dependency");

            migrationBuilder.DropTable(
                name: "UploadedPlugins");

            migrationBuilder.DropTable(
                name: "PluginVersions");

            migrationBuilder.DropTable(
                name: "Plugins");
        }
    }
}
