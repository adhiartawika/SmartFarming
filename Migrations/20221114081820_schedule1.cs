using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class schedule1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_ScheduleIntervals_ScheduleIntervalId",
                table: "Schedules");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleIntervalId",
                table: "Schedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_ScheduleIntervals_ScheduleIntervalId",
                table: "Schedules",
                column: "ScheduleIntervalId",
                principalTable: "ScheduleIntervals",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_ScheduleIntervals_ScheduleIntervalId",
                table: "Schedules");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleIntervalId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_ScheduleIntervals_ScheduleIntervalId",
                table: "Schedules",
                column: "ScheduleIntervalId",
                principalTable: "ScheduleIntervals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
