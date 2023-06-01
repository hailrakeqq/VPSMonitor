using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPSMonitor.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSSHKeysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refreshTokens",
                table: "refreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usersSSHKeys",
                table: "usersSSHKeys");

            migrationBuilder.RenameTable(
                name: "refreshTokens",
                newName: "refreshtokens");

            migrationBuilder.RenameTable(
                name: "usersSSHKeys",
                newName: "sshkeys");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "sshkeys",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_refreshtokens",
                table: "refreshtokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sshkeys",
                table: "sshkeys",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refreshtokens",
                table: "refreshtokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sshkeys",
                table: "sshkeys");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "sshkeys");

            migrationBuilder.RenameTable(
                name: "refreshtokens",
                newName: "refreshTokens");

            migrationBuilder.RenameTable(
                name: "sshkeys",
                newName: "usersSSHKeys");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refreshTokens",
                table: "refreshTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usersSSHKeys",
                table: "usersSSHKeys",
                column: "Id");
        }
    }
}
