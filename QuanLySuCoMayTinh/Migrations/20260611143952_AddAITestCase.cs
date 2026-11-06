using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class AddAITestCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AI_TEST_CASE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpectedFault = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RulePrediction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AIPredictionTop1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AIPredictionTop3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAICorrect = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_TEST_CASE", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AI_TEST_CASE");
        }
    }
}
