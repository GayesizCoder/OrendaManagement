using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSistemLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SistemLoglari",
                schema: "gayemkaratas_OrendaAdmin",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    IslemTipi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IslemDetayi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IPAdresi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SistemLoglari", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_SistemLoglari_Kisiler_KullaniciID",
                        column: x => x.KullaniciID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SistemLoglari_KullaniciID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "SistemLoglari",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SistemLoglari",
                schema: "gayemkaratas_OrendaAdmin");
        }
    }
}
