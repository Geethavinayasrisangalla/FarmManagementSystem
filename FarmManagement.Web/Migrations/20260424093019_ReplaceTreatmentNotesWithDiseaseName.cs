using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagement.Web.Migrations
{

    public partial class ReplaceTreatmentNotesWithDiseaseName : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreatmentNotes",
                table: "PestIncidents");

            migrationBuilder.AddColumn<string>(
                name: "DiseaseName",
                table: "PestIncidents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiseaseName",
                table: "PestIncidents");

            migrationBuilder.AddColumn<string>(
                name: "TreatmentNotes",
                table: "PestIncidents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
