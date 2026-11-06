using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class AddTopNMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Top3Accuracy",
                table: "ML_MODEL_INFO",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Top5Accuracy",
                table: "ML_MODEL_INFO",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Top3Accuracy",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "Top5Accuracy",
                table: "ML_MODEL_INFO");
        }
    }
}
