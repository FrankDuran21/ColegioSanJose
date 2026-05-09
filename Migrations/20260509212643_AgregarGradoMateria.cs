using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColegioSanJose.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGradoMateria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Grado",
                table: "Materias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grado",
                table: "Materias");
        }
    }
}
