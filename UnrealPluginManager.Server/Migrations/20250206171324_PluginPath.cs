using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class PluginPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PluginFileInfo",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginFileInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginFileInfo_Plugins_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Plugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plugins_Name_Version",
                table: "Plugins",
                columns: new[] { "Name", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PluginFileInfo_ParentId",
                table: "PluginFileInfo",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PluginFileInfo");

            migrationBuilder.DropIndex(
                name: "IX_Plugins_Name_Version",
                table: "Plugins");
        }
    }
}
