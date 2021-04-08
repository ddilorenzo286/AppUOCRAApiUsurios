using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LoginService.Migrations
{
    public partial class PrimeraMigracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(nullable: true),
                    Apellido = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Sal = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Perfil = table.Column<string>(nullable: true),
                    Ticket = table.Column<string>(nullable: true),
                    FechaCreacion = table.Column<DateTime>(nullable: true),
                    Foto = table.Column<string>(nullable: true),
                    Documento = table.Column<int>(nullable: false),
                    Telefono = table.Column<string>(nullable: true),
                    Sexo = table.Column<string>(nullable: true),
                    FechaNacimiento = table.Column<DateTime>(nullable: true),
                    LugarResidencia = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
