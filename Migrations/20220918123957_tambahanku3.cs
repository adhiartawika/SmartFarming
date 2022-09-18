using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class tambahanku3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Plants",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Plants",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Plants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Plants",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastModifiedBy",
                table: "Plants",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Plants");
        }
    }
}
