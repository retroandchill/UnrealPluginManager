using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToBinaryCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_plugin_versions_file_resources_source_id",
                table: "plugin_versions");

            migrationBuilder.DropTable(
                name: "plugin_binaries");

            migrationBuilder.DropIndex(
                name: "ix_plugin_versions_source_id",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "author_name",
                table: "plugins");

            migrationBuilder.DropColumn(
                name: "author_website",
                table: "plugins");

            migrationBuilder.DropColumn(
                name: "description",
                table: "plugins");

            migrationBuilder.DropColumn(
                name: "friendly_name",
                table: "plugins");

            migrationBuilder.DropColumn(
                name: "source_id",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "optional",
                table: "dependency");

            migrationBuilder.DropColumn(
                name: "type",
                table: "dependency");

            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "plugin_versions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "plugin_versions",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "homepage",
                table: "plugin_versions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "license",
                table: "plugin_versions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_sha",
                table: "plugin_versions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "source_url",
                table: "plugin_versions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "plugin_source_patches",
                columns: table => new
                {
                    plugin_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patch_number = table.Column<long>(type: "bigint", nullable: false),
                    file_resource_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin_source_patches", x => new { x.plugin_version_id, x.patch_number });
                    table.ForeignKey(
                        name: "fk_plugin_source_patches_file_resources_file_resource_id",
                        column: x => x.file_resource_id,
                        principalTable: "file_resources",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_plugin_source_patches_plugin_versions_plugin_version_id",
                        column: x => x.plugin_version_id,
                        principalTable: "plugin_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plugin_source_patches_file_resource_id",
                table: "plugin_source_patches",
                column: "file_resource_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plugin_source_patches");

            migrationBuilder.DropColumn(
                name: "author",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "description",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "homepage",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "license",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "source_sha",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "source_url",
                table: "plugin_versions");

            migrationBuilder.AddColumn<string>(
                name: "author_name",
                table: "plugins",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "author_website",
                table: "plugins",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "plugins",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "friendly_name",
                table: "plugins",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_id",
                table: "plugin_versions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "optional",
                table: "dependency",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "dependency",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "plugin_binaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    engine_version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    platform = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin_binaries", x => x.id);
                    table.ForeignKey(
                        name: "fk_plugin_binaries_file_resources_file_id",
                        column: x => x.file_id,
                        principalTable: "file_resources",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_plugin_binaries_plugin_versions_parent_id",
                        column: x => x.parent_id,
                        principalTable: "plugin_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plugin_versions_source_id",
                table: "plugin_versions",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_binaries_file_id",
                table: "plugin_binaries",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_binaries_parent_id",
                table: "plugin_binaries",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_binaries_parent_id_engine_version_platform",
                table: "plugin_binaries",
                columns: new[] { "parent_id", "engine_version", "platform" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_plugin_versions_file_resources_source_id",
                table: "plugin_versions",
                column: "source_id",
                principalTable: "file_resources",
                principalColumn: "id");
        }
    }
}
