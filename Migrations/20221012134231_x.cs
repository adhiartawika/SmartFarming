using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class x : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Datas_Parameters_ParameterId",
                table: "Datas");

            migrationBuilder.RenameColumn(
                name: "ParameterId",
                table: "Datas",
                newName: "ParentParamId");

            migrationBuilder.RenameIndex(
                name: "IX_Datas_ParameterId",
                table: "Datas",
                newName: "IX_Datas_ParentParamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Datas_ParentParameters_ParentParamId",
                table: "Datas",
                column: "ParentParamId",
                principalTable: "ParentParameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Datas_ParentParameters_ParentParamId",
                table: "Datas");

            migrationBuilder.RenameColumn(
                name: "ParentParamId",
                table: "Datas",
                newName: "ParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_Datas_ParentParamId",
                table: "Datas",
                newName: "IX_Datas_ParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Datas_Parameters_ParameterId",
                table: "Datas",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
