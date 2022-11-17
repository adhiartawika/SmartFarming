using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class schedule5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleOccurrences_Schedules_ScheduleId",
                table: "ScheduleOccurrences");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleOccurrences_ScheduleId",
                table: "ScheduleOccurrences");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOccurrences_ScheduleId",
                table: "ScheduleOccurrences",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleOccurrences_Schedules_ScheduleId",
                table: "ScheduleOccurrences",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
