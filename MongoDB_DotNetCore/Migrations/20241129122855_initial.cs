using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MongoDB_DotNetCore.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alarm_History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    L1Name = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    L0Name = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    number = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    enddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timespan = table.Column<double>(type: "float", nullable: false),
                    message = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    type = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    level = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarm_History", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1_Pool",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L1Name = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    updatedate = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false),
                    enddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timespan = table.Column<double>(type: "float", nullable: false),
                    signalname = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    value = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1_Pool", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L1Signal_Pool",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: false),
                    L1Name = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    enddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timespan = table.Column<double>(type: "float", nullable: false),
                    signalname = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    value = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1Signal_Pool", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LatestCapturedEndDate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    collectionName = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: true),
                    LatestCapturedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestCapturedEndDate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag_Pool",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ObjectId = table.Column<string>(type: "NVARCHAR(64)", maxLength: 64, nullable: true),
                    TagName = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    enddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timespan = table.Column<double>(type: "float", nullable: false),
                    signalname = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true),
                    value = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag_Pool", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alarm_History");

            migrationBuilder.DropTable(
                name: "L1_Pool");

            migrationBuilder.DropTable(
                name: "L1Signal_Pool");

            migrationBuilder.DropTable(
                name: "LatestCapturedEndDate");

            migrationBuilder.DropTable(
                name: "Tag_Pool");
        }
    }
}
