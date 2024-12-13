using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MongoDB_DotNetCore.Migrations
{
    /// <inheritdoc />
    public partial class initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "L1Signal_Pool_Capped",
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
                    table.PrimaryKey("PK_L1Signal_Pool_Capped", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "L1Signal_Pool_Capped");
        }
    }
}
