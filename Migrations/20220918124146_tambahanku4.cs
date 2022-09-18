using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class tambahanku4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Sensors",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Sensors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sensors",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Sensors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Sensors",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "Sensors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Parameters",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Parameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Parameters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Parameters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Parameters",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "Parameters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Actuators",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Actuators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Actuators",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Actuators",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Actuators",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "Actuators",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Parameters");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Actuators");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Actuators");
        }
    }
}
