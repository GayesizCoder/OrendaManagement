using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline migration - Database already contains these objects
            // Tables [Kisiler] and [ToDo] and schema [gayemkaratas_OrendaAdmin] are already present.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDo",
                schema: "gayemkaratas_OrendaAdmin");

            migrationBuilder.DropTable(
                name: "Kisiler",
                schema: "gayemkaratas_OrendaAdmin");
        }
    }
}
