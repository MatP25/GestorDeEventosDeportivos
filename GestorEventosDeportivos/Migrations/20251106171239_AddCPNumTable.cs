using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorEventosDeportivos.Migrations
{
    /// <inheritdoc />
    public partial class AddCPNumTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "NumeroCorredor",
                table: "Participaciones",
                type: "int unsigned",
                nullable: true,
                oldClrType: typeof(uint),
                oldType: "int unsigned");

            migrationBuilder.CreateTable(
                name: "CPNums",
                columns: table => new
                {
                    CarreraId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    NextNumber = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPNums", x => x.CarreraId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CPNums");

            migrationBuilder.AlterColumn<uint>(
                name: "NumeroCorredor",
                table: "Participaciones",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(uint),
                oldType: "int unsigned",
                oldNullable: true);
        }
    }
}
