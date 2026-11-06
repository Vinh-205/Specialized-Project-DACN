using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLySuCoMayTinh.Migrations
{
    /// <inheritdoc />
    public partial class AddHybridAI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LICH_SU_CHAN_DOAN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    MaLoiRule = table.Column<int>(type: "int", nullable: true),
                    MaLoiAI = table.Column<int>(type: "int", nullable: true),
                    MaLoiThucTe = table.Column<int>(type: "int", nullable: true),
                    TenLoiThucTeSnapshot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LICH_SU_CHAN_DOAN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LICH_SU_CHAN_DOAN_PHIEU_TIEP_NHAN_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PHIEU_TIEP_NHAN",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ML_MODEL_INFO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MicroAccuracy = table.Column<double>(type: "float", nullable: false),
                    MacroAccuracy = table.Column<double>(type: "float", nullable: false),
                    LogLoss = table.Column<double>(type: "float", nullable: false),
                    LogLossReduction = table.Column<double>(type: "float", nullable: false),
                    TrainingRecordCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ML_MODEL_INFO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MODEL_ACTIVATION_LOG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OldVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NewVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MODEL_ACTIVATION_LOG", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LICH_SU_CHAN_DOAN_TRIEU_CHUNG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLichSuChanDoan = table.Column<int>(type: "int", nullable: false),
                    MaTrieuChung = table.Column<int>(type: "int", nullable: false),
                    TenTrieuChungSnapshot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_LICH_SU_CHAN_DOAN_MaLichSuChanDoan",
                        column: x => x.MaLichSuChanDoan,
                        principalTable: "LICH_SU_CHAN_DOAN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_TRIEU_CHUNG_MaTrieuChung",
                        column: x => x.MaTrieuChung,
                        principalTable: "TRIEU_CHUNG",
                        principalColumn: "MaTC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AI_TRAINING_JOB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelInfoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_TRAINING_JOB", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AI_TRAINING_JOB_ML_MODEL_INFO_ModelInfoId",
                        column: x => x.ModelInfoId,
                        principalTable: "ML_MODEL_INFO",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AI_TRAINING_JOB_ModelInfoId",
                table: "AI_TRAINING_JOB",
                column: "ModelInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_CHAN_DOAN_MaPhieu",
                table: "LICH_SU_CHAN_DOAN",
                column: "MaPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaLichSuChanDoan",
                table: "LICH_SU_CHAN_DOAN_TRIEU_CHUNG",
                column: "MaLichSuChanDoan");

            migrationBuilder.CreateIndex(
                name: "IX_LICH_SU_CHAN_DOAN_TRIEU_CHUNG_MaTrieuChung",
                table: "LICH_SU_CHAN_DOAN_TRIEU_CHUNG",
                column: "MaTrieuChung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AI_TRAINING_JOB");

            migrationBuilder.DropTable(
                name: "LICH_SU_CHAN_DOAN_TRIEU_CHUNG");

            migrationBuilder.DropTable(
                name: "MODEL_ACTIVATION_LOG");

            migrationBuilder.DropTable(
                name: "ML_MODEL_INFO");

            migrationBuilder.DropTable(
                name: "LICH_SU_CHAN_DOAN");
        }
    }
}
