using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class tambahanku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Mikrokontrollers_MikrokontrollerId",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_MikrokontrollerId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "MikrokontrollerId",
                table: "Sensors");

            migrationBuilder.AddColumn<int>(
                name: "MikrocontrollerId",
                table: "Sensors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Parameters",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Mikrokontrollers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Mikrokontrollers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                table: "Lands",
                type: "longblob",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "longblob");

            migrationBuilder.CreateTable(
                name: "Actuators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MikrocontrollerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actuators_Mikrokontrollers_MikrocontrollerId",
                        column: x => x.MikrocontrollerId,
                        principalTable: "Mikrokontrollers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MikrocontrollerId",
                table: "Sensors",
                column: "MikrocontrollerId");

            migrationBuilder.CreateIndex(
                name: "IX_Actuators_MikrocontrollerId",
                table: "Actuators",
                column: "MikrocontrollerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Mikrokontrollers_MikrocontrollerId",
                table: "Sensors",
                column: "MikrocontrollerId",
                principalTable: "Mikrokontrollers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensors_Mikrokontrollers_MikrocontrollerId",
                table: "Sensors");

            migrationBuilder.DropTable(
                name: "Actuators");

            migrationBuilder.DropIndex(
                name: "IX_Sensors_MikrocontrollerId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "MikrocontrollerId",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Mikrokontrollers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Mikrokontrollers");

            migrationBuilder.AddColumn<int>(
                name: "MikrokontrollerId",
                table: "Sensors",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "Photo",
                table: "Lands",
                type: "longblob",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "longblob",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_MikrokontrollerId",
                table: "Sensors",
                column: "MikrokontrollerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensors_Mikrokontrollers_MikrokontrollerId",
                table: "Sensors",
                column: "MikrokontrollerId",
                principalTable: "Mikrokontrollers",
                principalColumn: "Id");
        }
    }
}
