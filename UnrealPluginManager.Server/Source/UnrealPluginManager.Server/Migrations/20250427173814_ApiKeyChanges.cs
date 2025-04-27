using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnrealPluginManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class ApiKeyChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "key",
                table: "api_keys",
                newName: "display_name");

            migrationBuilder.AddColumn<Guid>(
                name: "external_id",
                table: "api_keys",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_api_keys_external_id",
                table: "api_keys",
                column: "external_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_api_keys_external_id",
                table: "api_keys");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "api_keys");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "api_keys",
                newName: "key");
        }
    }
}
