using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewTablePaymentsAndAddedFieldToWorkedHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasPayed",
                table: "ContructionSiteWorkedHoursWorkers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContructionSiteWorkedHoursWorkerId = table.Column<int>(type: "int", nullable: false),
                    PayedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_ContructionSiteWorkedHoursWorkers_ContructionSiteWorkedHoursWorkerId",
                        column: x => x.ContructionSiteWorkedHoursWorkerId,
                        principalTable: "ContructionSiteWorkedHoursWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ContructionSiteWorkedHoursWorkerId",
                table: "Payments",
                column: "ContructionSiteWorkedHoursWorkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropColumn(
                name: "WasPayed",
                table: "ContructionSiteWorkedHoursWorkers");
        }
    }
}
