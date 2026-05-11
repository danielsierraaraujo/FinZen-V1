using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinZen.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Transacciones");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Transacciones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { 1, "Comida", "Gasto" },
                    { 2, "Transporte", "Gasto" },
                    { 3, "Entretenimiento", "Gasto" },
                    { 4, "Salud", "Gasto" },
                    { 5, "Educación", "Gasto" },
                    { 6, "Servicios", "Gasto" },
                    { 7, "Salario", "Ingreso" },
                    { 8, "Freelance", "Ingreso" },
                    { 9, "Inversión", "Ingreso" },
                    { 10, "Otros", "Ambos" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacciones_CategoriaId",
                table: "Transacciones",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Categorias_CategoriaId",
                table: "Transacciones",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Categorias_CategoriaId",
                table: "Transacciones");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Transacciones_CategoriaId",
                table: "Transacciones");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Transacciones");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Transacciones",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
