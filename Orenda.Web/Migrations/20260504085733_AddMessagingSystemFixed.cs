using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagingSystemFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GlobalID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SirketKodu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Mesajlar",
                columns: table => new
                {
                    MesajID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GonderenID = table.Column<int>(type: "int", nullable: false),
                    AliciID = table.Column<int>(type: "int", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    FotografUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GonderilmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesajlar", x => x.MesajID);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Kisiler_AliciID",
                        column: x => x.AliciID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Kisiler_GonderenID",
                        column: x => x.GonderenID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SohbetIstekleri",
                columns: table => new
                {
                    IstekID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GonderenID = table.Column<int>(type: "int", nullable: false),
                    AliciID = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SohbetIstekleri", x => x.IstekID);
                    table.ForeignKey(
                        name: "FK_SohbetIstekleri_Kisiler_AliciID",
                        column: x => x.AliciID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SohbetIstekleri_Kisiler_GonderenID",
                        column: x => x.GonderenID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_AliciID",
                table: "Mesajlar",
                column: "AliciID");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GonderenID",
                table: "Mesajlar",
                column: "GonderenID");

            migrationBuilder.CreateIndex(
                name: "IX_SohbetIstekleri_AliciID",
                table: "SohbetIstekleri",
                column: "AliciID");

            migrationBuilder.CreateIndex(
                name: "IX_SohbetIstekleri_GonderenID",
                table: "SohbetIstekleri",
                column: "GonderenID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mesajlar");

            migrationBuilder.DropTable(
                name: "SohbetIstekleri");

            migrationBuilder.DropColumn(
                name: "GlobalID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropColumn(
                name: "SirketKodu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");
        }
    }
}
