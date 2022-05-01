using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestWeb.Data.Migrations
{
    public partial class InitializeDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CreatorId",
                table: "Applications",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_CreatorId",
                table: "Applications",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_CreatorId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_CreatorId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Applications");
        }
    }
}
