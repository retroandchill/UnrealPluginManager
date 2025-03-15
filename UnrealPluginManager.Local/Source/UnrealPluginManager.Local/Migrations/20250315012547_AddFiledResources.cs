using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Local.Migrations
{
    /// <inheritdoc />
    public partial class AddFiledResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "plugins",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "plugin_versions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "icon_id",
                table: "plugin_versions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "readme_id",
                table: "plugin_versions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "source_id",
                table: "plugin_versions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "plugin_binaries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "file_id",
                table: "plugin_binaries",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "file_resources",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    original_filename = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    file_path = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_resources", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plugin_versions_icon_id",
                table: "plugin_versions",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_versions_readme_id",
                table: "plugin_versions",
                column: "readme_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_versions_source_id",
                table: "plugin_versions",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_binaries_file_id",
                table: "plugin_binaries",
                column: "file_id");

            migrationBuilder.AddForeignKey(
                name: "fk_plugin_binaries_file_resources_file_id",
                table: "plugin_binaries",
                column: "file_id",
                principalTable: "file_resources",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_plugin_versions_file_resources_icon_id",
                table: "plugin_versions",
                column: "icon_id",
                principalTable: "file_resources",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_plugin_versions_file_resources_readme_id",
                table: "plugin_versions",
                column: "readme_id",
                principalTable: "file_resources",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_plugin_versions_file_resources_source_id",
                table: "plugin_versions",
                column: "source_id",
                principalTable: "file_resources",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_plugin_binaries_file_resources_file_id",
                table: "plugin_binaries");

            migrationBuilder.DropForeignKey(
                name: "fk_plugin_versions_file_resources_icon_id",
                table: "plugin_versions");

            migrationBuilder.DropForeignKey(
                name: "fk_plugin_versions_file_resources_readme_id",
                table: "plugin_versions");

            migrationBuilder.DropForeignKey(
                name: "fk_plugin_versions_file_resources_source_id",
                table: "plugin_versions");

            migrationBuilder.DropTable(
                name: "file_resources");

            migrationBuilder.DropIndex(
                name: "ix_plugin_versions_icon_id",
                table: "plugin_versions");

            migrationBuilder.DropIndex(
                name: "ix_plugin_versions_readme_id",
                table: "plugin_versions");

            migrationBuilder.DropIndex(
                name: "ix_plugin_versions_source_id",
                table: "plugin_versions");

            migrationBuilder.DropIndex(
                name: "ix_plugin_binaries_file_id",
                table: "plugin_binaries");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "plugins");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "icon_id",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "readme_id",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "source_id",
                table: "plugin_versions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "plugin_binaries");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "plugin_binaries");
        }
    }
}
