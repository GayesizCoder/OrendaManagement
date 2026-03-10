using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseSifreLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SifreHash",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SifreHash",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
