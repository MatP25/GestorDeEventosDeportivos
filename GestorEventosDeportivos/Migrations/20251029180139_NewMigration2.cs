using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorEventosDeportivos.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CantidadParticipantes",
                table: "Eventos",
                newName: "CapacidadParticipantes");

            migrationBuilder.AddColumn<int>(
                name: "EstadoPago",
                table: "Participaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "CantidadParticipacionesPagas",
                table: "Carreras",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoPago",
                table: "Participaciones");

            migrationBuilder.DropColumn(
                name: "CantidadParticipacionesPagas",
                table: "Carreras");

            migrationBuilder.RenameColumn(
                name: "CapacidadParticipantes",
                table: "Eventos",
                newName: "CantidadParticipantes");
        }
    }
}
