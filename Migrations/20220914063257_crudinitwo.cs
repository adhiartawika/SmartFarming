using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class crudinitwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Regions_RegionId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_RegionId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Plants");

            migrationBuilder.CreateTable(
                name: "RegionPlant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPlant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionPlant_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegionPlant_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPlant_PlantId",
                table: "RegionPlant",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPlant_RegionId",
                table: "RegionPlant",
                column: "RegionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionPlant");

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Plants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_RegionId",
                table: "Plants",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Regions_RegionId",
                table: "Plants",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
