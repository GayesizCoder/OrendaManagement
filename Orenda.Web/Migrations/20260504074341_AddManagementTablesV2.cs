using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddManagementTablesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cihazlar",
                columns: table => new
                {
                    CihazID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CihazAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeriNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tür = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AtananCalisanID = table.Column<int>(type: "int", nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cihazlar", x => x.CihazID);
                    table.ForeignKey(
                        name: "FK_Cihazlar_Kisiler_AtananCalisanID",
                        column: x => x.AtananCalisanID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID");
                });

            migrationBuilder.CreateTable(
                name: "Izinler",
                columns: table => new
                {
                    IzinID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanID = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sebep = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YöneticiNotu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Izinler", x => x.IzinID);
                    table.ForeignKey(
                        name: "FK_Izinler_Kisiler_CalisanID",
                        column: x => x.CalisanID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Talepler",
                columns: table => new
                {
                    TalepID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanID = table.Column<int>(type: "int", nullable: false),
                    Tür = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Konu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Yanit = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talepler", x => x.TalepID);
                    table.ForeignKey(
                        name: "FK_Talepler_Kisiler_CalisanID",
                        column: x => x.CalisanID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cihazlar_AtananCalisanID",
                table: "Cihazlar",
                column: "AtananCalisanID");

            migrationBuilder.CreateIndex(
                name: "IX_Izinler_CalisanID",
                table: "Izinler",
                column: "CalisanID");

            migrationBuilder.CreateIndex(
                name: "IX_Talepler_CalisanID",
                table: "Talepler",
                column: "CalisanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cihazlar");

            migrationBuilder.DropTable(
                name: "Izinler");

            migrationBuilder.DropTable(
                name: "Talepler");
        }
    }
}
