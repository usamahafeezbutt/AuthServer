using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthServer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDelteInApplicationUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationToken_AspNetUsers_UserId",
                table: "ApplicationToken");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationToken_AspNetUsers_UserId",
                table: "ApplicationToken",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationToken_AspNetUsers_UserId",
                table: "ApplicationToken");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationToken_AspNetUsers_UserId",
                table: "ApplicationToken",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
