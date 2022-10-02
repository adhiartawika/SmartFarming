using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class updateaa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actuators_TypeActuators_TypeActuatorsId",
                table: "Actuators");

            migrationBuilder.DropIndex(
                name: "IX_Actuators_TypeActuatorsId",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "TypeActuatorsId",
                table: "Actuators");

            migrationBuilder.CreateIndex(
                name: "IX_Actuators_ActuatorTypeId",
                table: "Actuators",
                column: "ActuatorTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actuators_TypeActuators_ActuatorTypeId",
                table: "Actuators",
                column: "ActuatorTypeId",
                principalTable: "TypeActuators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actuators_TypeActuators_ActuatorTypeId",
                table: "Actuators");

            migrationBuilder.DropIndex(
                name: "IX_Actuators_ActuatorTypeId",
                table: "Actuators");

            migrationBuilder.AddColumn<int>(
                name: "TypeActuatorsId",
                table: "Actuators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Actuators_TypeActuatorsId",
                table: "Actuators",
                column: "TypeActuatorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actuators_TypeActuators_TypeActuatorsId",
                table: "Actuators",
                column: "TypeActuatorsId",
                principalTable: "TypeActuators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
