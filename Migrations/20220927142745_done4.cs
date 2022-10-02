using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class done4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mikrokontrollers_Regions_RegionId",
                table: "Mikrokontrollers");

            migrationBuilder.DropIndex(
                name: "IX_Mikrokontrollers_RegionId",
                table: "Mikrokontrollers");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Mikrokontrollers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Mikrokontrollers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mikrokontrollers_RegionId",
                table: "Mikrokontrollers",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mikrokontrollers_Regions_RegionId",
                table: "Mikrokontrollers",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
