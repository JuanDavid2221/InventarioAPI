using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventarioAPI.Migrations
{
    /// <inheritdoc />
    public partial class EstructuraFinalJuanDavid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Usuarios",
                newName: "IdEmpresa");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_EmpresaId",
                table: "Usuarios",
                newName: "IX_Usuarios_IdEmpresa");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Empresas",
                newName: "TipoNegocio");

            migrationBuilder.RenameColumn(
                name: "Nit",
                table: "Empresas",
                newName: "NIT_RUT");

            migrationBuilder.AlterColumn<int>(
                name: "RolId",
                table: "Usuarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorreoContacto",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Empresas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Empresas_IdEmpresa",
                table: "Usuarios",
                column: "IdEmpresa",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Empresas_IdEmpresa",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CorreoContacto",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Empresas");

            migrationBuilder.RenameColumn(
                name: "IdEmpresa",
                table: "Usuarios",
                newName: "EmpresaId");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdEmpresa",
                table: "Usuarios",
                newName: "IX_Usuarios_EmpresaId");

            migrationBuilder.RenameColumn(
                name: "TipoNegocio",
                table: "Empresas",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "NIT_RUT",
                table: "Empresas",
                newName: "Nit");

            migrationBuilder.AlterColumn<int>(
                name: "RolId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
