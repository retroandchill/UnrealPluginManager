using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialAdd : Migration
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
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dependency",
                columns: table => new
                {
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Optional = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependency", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_Dependency_Plugins_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dependency_Plugins_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EngineVersion",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Major = table.Column<uint>(type: "INTEGER", nullable: false),
                    Minor = table.Column<uint>(type: "INTEGER", nullable: false),
                    PluginId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineVersion_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PluginAuthor",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PluginId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorWebsite = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginAuthor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginAuthor_Plugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dependency_ChildId",
                table: "Dependency",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Dependency_ParentId",
                table: "Dependency",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineVersion_PluginId_Major_Minor",
                table: "EngineVersion",
                columns: new[] { "PluginId", "Major", "Minor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PluginAuthor_AuthorName",
                table: "PluginAuthor",
                column: "AuthorName");

            migrationBuilder.CreateIndex(
                name: "IX_PluginAuthor_PluginId_AuthorName",
                table: "PluginAuthor",
                columns: new[] { "PluginId", "AuthorName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_Name",
                table: "Plugins",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_Type",
                table: "Plugins",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dependency");

            migrationBuilder.DropTable(
                name: "EngineVersion");

            migrationBuilder.DropTable(
                name: "PluginAuthor");

            migrationBuilder.DropTable(
                name: "Plugins");
        }
    }
}
