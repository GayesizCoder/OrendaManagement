using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddOnayDurumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnayDurumu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OnayNotu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnayDurumu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");

            migrationBuilder.DropColumn(
                name: "OnayNotu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");
        }
    }
}
