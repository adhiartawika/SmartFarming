using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class tambahanku7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SensorId",
                table: "Datas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Datas_SensorId",
                table: "Datas",
                column: "SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Datas_Sensors_SensorId",
                table: "Datas",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Datas_Sensors_SensorId",
                table: "Datas");

            migrationBuilder.DropIndex(
                name: "IX_Datas_SensorId",
                table: "Datas");

            migrationBuilder.DropColumn(
                name: "SensorId",
                table: "Datas");
        }
    }
}
