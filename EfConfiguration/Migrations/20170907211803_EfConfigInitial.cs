using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace EfConfiguration.Migrations
{
    public partial class EfConfigInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "cfg");

            migrationBuilder.CreateTable(
                name: "ConfigurationValues",
                schema: "cfg",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationValues", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurationValues_LastUpdated",
                schema: "cfg",
                table: "ConfigurationValues",
                column: "LastUpdated");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationValues",
                schema: "cfg");
        }
    }
}
