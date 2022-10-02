using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class done5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MiniPcId",
                table: "IdentityIoTs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityIoTs_MiniPcId",
                table: "IdentityIoTs",
                column: "MiniPcId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityIoTs_MiniPcs_MiniPcId",
                table: "IdentityIoTs",
                column: "MiniPcId",
                principalTable: "MiniPcs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdentityIoTs_MiniPcs_MiniPcId",
                table: "IdentityIoTs");

            migrationBuilder.DropIndex(
                name: "IX_IdentityIoTs_MiniPcId",
                table: "IdentityIoTs");

            migrationBuilder.DropColumn(
                name: "MiniPcId",
                table: "IdentityIoTs");
        }
    }
}
