using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_Test.Migrations
{
    public partial class updatemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "images",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "images");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "images");
        }
    }
}
