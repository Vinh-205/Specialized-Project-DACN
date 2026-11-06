using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class HybridAI_DataStrategy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaLoiAI",
                table: "LICH_SU_CHAN_DOAN",
                newName: "MaLoiAITop3");

            migrationBuilder.AddColumn<double>(
                name: "AIAccuracy",
                table: "ML_MODEL_INFO",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ConfusionMatrixJson",
                table: "ML_MODEL_INFO",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatasetGeneratedAt",
                table: "ML_MODEL_INFO",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DatasetVersion",
                table: "ML_MODEL_INFO",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealRecordsCount",
                table: "ML_MODEL_INFO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RuleAccuracy",
                table: "ML_MODEL_INFO",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SyntheticRecordsCount",
                table: "ML_MODEL_INFO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserHistoryRecordsCount",
                table: "ML_MODEL_INFO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "ConfidenceTop1",
                table: "LICH_SU_CHAN_DOAN",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ConfidenceTop2",
                table: "LICH_SU_CHAN_DOAN",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "ConfidenceTop3",
                table: "LICH_SU_CHAN_DOAN",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataSource",
                table: "LICH_SU_CHAN_DOAN",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaLoiAITop1",
                table: "LICH_SU_CHAN_DOAN",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaLoiAITop2",
                table: "LICH_SU_CHAN_DOAN",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIAccuracy",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "ConfusionMatrixJson",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "DatasetGeneratedAt",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "DatasetVersion",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "RealRecordsCount",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "RuleAccuracy",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "SyntheticRecordsCount",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "UserHistoryRecordsCount",
                table: "ML_MODEL_INFO");

            migrationBuilder.DropColumn(
                name: "ConfidenceTop1",
                table: "LICH_SU_CHAN_DOAN");

            migrationBuilder.DropColumn(
                name: "ConfidenceTop2",
                table: "LICH_SU_CHAN_DOAN");

            migrationBuilder.DropColumn(
                name: "ConfidenceTop3",
                table: "LICH_SU_CHAN_DOAN");

            migrationBuilder.DropColumn(
                name: "DataSource",
                table: "LICH_SU_CHAN_DOAN");

            migrationBuilder.DropColumn(
                name: "MaLoiAITop1",
                table: "LICH_SU_CHAN_DOAN");

            migrationBuilder.DropColumn(
                name: "MaLoiAITop2",
                table: "LICH_SU_CHAN_DOAN");

            migrationBuilder.RenameColumn(
                name: "MaLoiAITop3",
                table: "LICH_SU_CHAN_DOAN",
                newName: "MaLoiAI");
        }
    }
}
