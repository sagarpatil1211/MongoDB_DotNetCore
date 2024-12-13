using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MongoDB_DotNetCore.Migrations
{
    /// <inheritdoc />
    public partial class initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "Tag_Pool",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "Tag_Pool",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LatestCapturedEndDate",
                table: "LatestCapturedEndDate",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "L1Signal_Pool_Capped",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<double>(
                name: "timespan",
                table: "L1Signal_Pool_Capped",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "L1Signal_Pool_Capped",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "L1Signal_Pool",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "L1Signal_Pool",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "updatedate",
                table: "L1_Pool",
                type: "NVARCHAR(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<double>(
                name: "timespan",
                table: "L1_Pool",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "L1_Pool",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "Alarm_History",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "Alarm_History",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Operator_History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1Name = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    updatedate = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    enddate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    timespan = table.Column<double>(type: "float", nullable: true),
                    signalname = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    value = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operator_History", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductResult_History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1Name = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    productname = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    updatedate = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    enddate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    timespan = table.Column<double>(type: "float", nullable: true),
                    productresult = table.Column<int>(type: "int", nullable: false),
                    productresult_accumulate = table.Column<int>(type: "int", nullable: false),
                    resultflag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductResult_History", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemDiagnosis_Machine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    PCName = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    MachineName = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CycleTime = table.Column<int>(type: "int", nullable: true),
                    ExecuteTime = table.Column<int>(type: "int", nullable: true),
                    Updatedate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDiagnosis_Machine", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operator_History");

            migrationBuilder.DropTable(
                name: "ProductResult_History");

            migrationBuilder.DropTable(
                name: "SystemDiagnosis_Machine");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "Tag_Pool",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "Tag_Pool",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LatestCapturedEndDate",
                table: "LatestCapturedEndDate",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "L1Signal_Pool_Capped",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "timespan",
                table: "L1Signal_Pool_Capped",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "L1Signal_Pool_Capped",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "L1Signal_Pool",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "L1Signal_Pool",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updatedate",
                table: "L1_Pool",
                type: "NVARCHAR(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "timespan",
                table: "L1_Pool",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "L1_Pool",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedate",
                table: "Alarm_History",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "enddate",
                table: "Alarm_History",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
