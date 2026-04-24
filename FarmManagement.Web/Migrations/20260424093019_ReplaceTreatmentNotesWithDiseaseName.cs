using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTreatmentNotesWithDiseaseName : Migration
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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
