using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plugins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    friendly_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    author_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    author_website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plugin_versions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_string = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    major = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    minor = table.Column<int>(type: "integer", nullable: false),
                    patch = table.Column<int>(type: "integer", nullable: false),
                    prerelease = table.Column<string>(type: "text", nullable: true),
                    prerelease_number = table.Column<int>(type: "integer", nullable: true),
                    metadata = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin_versions", x => x.id);
                    table.ForeignKey(
                        name: "fk_plugin_versions_plugins_parent_id",
                        column: x => x.parent_id,
                        principalTable: "plugins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dependency",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plugin_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    plugin_version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    optional = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependency", x => x.id);
                    table.ForeignKey(
                        name: "fk_dependency_plugin_versions_parent_id",
                        column: x => x.parent_id,
                        principalTable: "plugin_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plugin_binaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    platform = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    engine_version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin_binaries", x => x.id);
                    table.ForeignKey(
                        name: "fk_plugin_binaries_plugin_versions_parent_id",
                        column: x => x.parent_id,
                        principalTable: "plugin_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dependency_parent_id",
                table: "dependency",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_binaries_parent_id",
                table: "plugin_binaries",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_binaries_parent_id_engine_version_platform",
                table: "plugin_binaries",
                columns: new[] { "parent_id", "engine_version", "platform" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plugin_versions_parent_id",
                table: "plugin_versions",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugins_name",
                table: "plugins",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dependency");

            migrationBuilder.DropTable(
                name: "plugin_binaries");

            migrationBuilder.DropTable(
                name: "plugin_versions");

            migrationBuilder.DropTable(
                name: "plugins");
        }
    }
}
