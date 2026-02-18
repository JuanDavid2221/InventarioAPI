using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioAPI.Migrations
{
    public partial class AgregarSoloNotificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Solo dejamos la creación de la tabla Notificaciones
            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensaje = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Solo eliminamos la tabla Notificaciones si se revierte la migración
            migrationBuilder.DropTable(
                name: "Notificaciones");
        }
    }
}