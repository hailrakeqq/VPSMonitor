using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPSMonitor.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSshKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SSHKey",
                table: "sshkeys",
                newName: "Ssh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ssh",
                table: "sshkeys",
                newName: "SSHKey");
        }
    }
}
