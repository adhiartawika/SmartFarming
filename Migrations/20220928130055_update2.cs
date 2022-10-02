using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actuators_MiniPcs_MiniPcId",
                table: "Actuators");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_MiniPcs_MiniPcId",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_MiniPcId",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Actuators_MiniPcId",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "MiniPcId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "IotId",
                table: "MiniPcs");

            migrationBuilder.DropColumn(
                name: "MiniPcId",
                table: "Actuators");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MiniPcId",
                table: "Sensors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IotId",
                table: "MiniPcs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MiniPcId",
                table: "Actuators",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MiniPcId",
                table: "Sensors",
                column: "MiniPcId");

            migrationBuilder.CreateIndex(
                name: "IX_Actuators_MiniPcId",
                table: "Actuators",
                column: "MiniPcId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actuators_MiniPcs_MiniPcId",
                table: "Actuators",
                column: "MiniPcId",
                principalTable: "MiniPcs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_MiniPcs_MiniPcId",
                table: "Sensors",
                column: "MiniPcId",
                principalTable: "MiniPcs",
                principalColumn: "Id");
        }
    }
}
