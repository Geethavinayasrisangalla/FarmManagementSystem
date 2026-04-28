using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagement.Web.Migrations
{

    public partial class AddIsBlockedToAppUser : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "AppUsers");
        }
    }
}
