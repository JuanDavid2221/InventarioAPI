using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioAPI.Migrations
{
    /// <inheritdoc />
    public partial class UnicidadNitProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NIT",
                table: "Proveedores",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_NIT_IdEmpresa",
                table: "Proveedores",
                columns: new[] { "NIT", "IdEmpresa" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Proveedores_NIT_IdEmpresa",
                table: "Proveedores");

            migrationBuilder.AlterColumn<string>(
                name: "NIT",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
