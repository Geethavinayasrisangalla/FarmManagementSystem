using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmManagement.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPestIncidentIdToResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PestIncidentId",
                table: "Resources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_PestIncidentId",
                table: "Resources",
                column: "PestIncidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_PestIncidents_PestIncidentId",
                table: "Resources",
                column: "PestIncidentId",
                principalTable: "PestIncidents",
                principalColumn: "PestIncidentId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_PestIncidents_PestIncidentId",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_PestIncidentId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "PestIncidentId",
                table: "Resources");
        }
    }
}
