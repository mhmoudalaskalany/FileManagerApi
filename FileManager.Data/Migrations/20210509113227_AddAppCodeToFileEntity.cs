using Microsoft.EntityFrameworkCore.Migrations;

namespace FileManager.Data.Migrations
{
    public partial class AddAppCodeToFileEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppCode",
                table: "Files",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppCode",
                table: "Files");
        }
    }
}
