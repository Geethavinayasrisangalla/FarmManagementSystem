using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagement.Web.Migrations
{

    public partial class AddPasswordHint : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHint",
                table: "AppUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHint",
                table: "AppUsers");
        }
    }
}
