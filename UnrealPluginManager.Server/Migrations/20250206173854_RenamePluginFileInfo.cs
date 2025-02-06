using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenamePluginFileInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PluginFileInfo_Plugins_ParentId",
                table: "PluginFileInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PluginFileInfo",
                table: "PluginFileInfo");

            migrationBuilder.RenameTable(
                name: "PluginFileInfo",
                newName: "UploadedPlugins");

            migrationBuilder.RenameIndex(
                name: "IX_PluginFileInfo_ParentId",
                table: "UploadedPlugins",
                newName: "IX_UploadedPlugins_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UploadedPlugins",
                table: "UploadedPlugins",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedPlugins_Plugins_ParentId",
                table: "UploadedPlugins",
                column: "ParentId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedPlugins_Plugins_ParentId",
                table: "UploadedPlugins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UploadedPlugins",
                table: "UploadedPlugins");

            migrationBuilder.RenameTable(
                name: "UploadedPlugins",
                newName: "PluginFileInfo");

            migrationBuilder.RenameIndex(
                name: "IX_UploadedPlugins_ParentId",
                table: "PluginFileInfo",
                newName: "IX_PluginFileInfo_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PluginFileInfo",
                table: "PluginFileInfo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PluginFileInfo_Plugins_ParentId",
                table: "PluginFileInfo",
                column: "ParentId",
                principalTable: "Plugins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
