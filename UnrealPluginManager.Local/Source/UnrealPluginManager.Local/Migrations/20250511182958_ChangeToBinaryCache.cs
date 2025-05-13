using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Local.Migrations
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
                name: "allowed_plugins");

            migrationBuilder.DropTable(
                name: "plugin_binaries");

            migrationBuilder.DropTable(
                name: "plugin_owners");

            migrationBuilder.DropTable(
                name: "api_keys");

            migrationBuilder.DropTable(
                name: "users");

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
                name: "optional",
                table: "dependency");

            migrationBuilder.DropColumn(
                name: "type",
                table: "dependency");

            migrationBuilder.RenameColumn(
                name: "source_id",
                table: "plugin_versions",
                newName: "source_url");

            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "plugin_versions",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "plugin_versions",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "homepage",
                table: "plugin_versions",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "license",
                table: "plugin_versions",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_sha",
                table: "plugin_versions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "cached_builds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    plugin_version_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    engine_version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    directory_name = table.Column<string>(type: "TEXT", nullable: false),
                    built_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cached_builds", x => x.id);
                    table.ForeignKey(
                        name: "fk_cached_builds_plugin_versions_plugin_version_id",
                        column: x => x.plugin_version_id,
                        principalTable: "plugin_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plugin_source_patches",
                columns: table => new
                {
                    plugin_version_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    patch_number = table.Column<uint>(type: "INTEGER", nullable: false),
                    file_resource_id = table.Column<Guid>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "dependency_build_versions",
                columns: table => new
                {
                    build_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    dependency_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependency_build_versions", x => new { x.build_id, x.dependency_id });
                    table.ForeignKey(
                        name: "fk_dependency_build_versions_cached_builds_build_id",
                        column: x => x.build_id,
                        principalTable: "cached_builds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dependency_build_versions_dependency_dependency_id",
                        column: x => x.dependency_id,
                        principalTable: "dependency",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plugin_build_platforms",
                columns: table => new
                {
                    build_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    platform = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin_build_platforms", x => new { x.build_id, x.platform });
                    table.ForeignKey(
                        name: "fk_plugin_build_platforms_cached_builds_build_id",
                        column: x => x.build_id,
                        principalTable: "cached_builds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cached_builds_plugin_version_id",
                table: "cached_builds",
                column: "plugin_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_dependency_build_versions_dependency_id",
                table: "dependency_build_versions",
                column: "dependency_id");

            migrationBuilder.CreateIndex(
                name: "ix_plugin_source_patches_file_resource_id",
                table: "plugin_source_patches",
                column: "file_resource_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dependency_build_versions");

            migrationBuilder.DropTable(
                name: "plugin_build_platforms");

            migrationBuilder.DropTable(
                name: "plugin_source_patches");

            migrationBuilder.DropTable(
                name: "cached_builds");

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

            migrationBuilder.RenameColumn(
                name: "source_url",
                table: "plugin_versions",
                newName: "source_id");

            migrationBuilder.AddColumn<string>(
                name: "author_name",
                table: "plugins",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "author_website",
                table: "plugins",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "plugins",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "friendly_name",
                table: "plugins",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "optional",
                table: "dependency",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "dependency",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "plugin_binaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    file_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    parent_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    engine_version = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    platform = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    profile_picture_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "TEXT", maxLength: 31, nullable: true),
                    username = table.Column<string>(type: "TEXT", maxLength: 31, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_file_resources_profile_picture_id",
                        column: x => x.profile_picture_id,
                        principalTable: "file_resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    display_name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    external_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    plugin_glob = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_keys", x => x.id);
                    table.ForeignKey(
                        name: "fk_api_keys_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plugin_owners",
                columns: table => new
                {
                    owner_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    plugin_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plugin_owners", x => new { x.owner_id, x.plugin_id });
                    table.ForeignKey(
                        name: "fk_plugin_owners_plugins_plugin_id",
                        column: x => x.plugin_id,
                        principalTable: "plugins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_plugin_owners_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "allowed_plugins",
                columns: table => new
                {
                    api_key_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    plugin_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_allowed_plugins", x => new { x.api_key_id, x.plugin_id });
                    table.ForeignKey(
                        name: "fk_allowed_plugins_api_keys_api_key_id",
                        column: x => x.api_key_id,
                        principalTable: "api_keys",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_allowed_plugins_plugins_plugin_id",
                        column: x => x.plugin_id,
                        principalTable: "plugins",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plugin_versions_source_id",
                table: "plugin_versions",
                column: "source_id");

            migrationBuilder.CreateIndex(
                name: "ix_allowed_plugins_plugin_id",
                table: "allowed_plugins",
                column: "plugin_id");

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_external_id",
                table: "api_keys",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_user_id",
                table: "api_keys",
                column: "user_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_plugin_owners_plugin_id",
                table: "plugin_owners",
                column: "plugin_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_profile_picture_id",
                table: "users",
                column: "profile_picture_id");

            migrationBuilder.AddForeignKey(
                name: "fk_plugin_versions_file_resources_source_id",
                table: "plugin_versions",
                column: "source_id",
                principalTable: "file_resources",
                principalColumn: "id");
        }
    }
}
