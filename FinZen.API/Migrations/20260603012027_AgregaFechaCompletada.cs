using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinZen.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregaFechaCompletada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompletada",
                table: "Metas",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCompletada",
                table: "Metas");
        }
    }
}
