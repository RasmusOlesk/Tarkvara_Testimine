using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TARge25Shop.Data.Migrations
{
    public partial class CreateSpaceshipsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spaceships",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),
                    ShipType = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),
                    Crew = table.Column<int>(
                        type: "int",
                        nullable: false),
                    EnginePower = table.Column<int>(
                        type: "int",
                        nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),
                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaceships", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spaceships");
        }
    }
}