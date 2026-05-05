using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUserHireDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "IseBaslamaTarihi",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "YillikIzinHakki",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IseBaslamaTarihi",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropColumn(
                name: "YillikIzinHakki",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");
        }
    }
}
