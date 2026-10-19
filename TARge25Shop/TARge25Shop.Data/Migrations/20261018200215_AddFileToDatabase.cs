
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TARge25Shop.Data.Migrations
{
    public partial class AddFileToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileToDatabases",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false),

                    ImageTitle = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true),

                    ImageData = table.Column<byte[]>(
                        type: "varbinary(max)",
                        nullable: true),

                    KindergartenId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileToDatabases", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileToDatabases");
        }
    }
}
