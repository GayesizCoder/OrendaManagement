using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSaglikVerisiVeTakimlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaglikVerileri",
                schema: "gayemkaratas_OrendaAdmin",
                columns: table => new
                {
                    SaglikVerisiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanID = table.Column<int>(type: "int", nullable: false),
                    TarihSaat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nabiz = table.Column<int>(type: "int", nullable: false),
                    AdimSayisi = table.Column<int>(type: "int", nullable: false),
                    UykuSaati = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaglikVerileri", x => x.SaglikVerisiID);
                    table.ForeignKey(
                        name: "FK_SaglikVerileri_Kisiler_CalisanID",
                        column: x => x.CalisanID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Kisiler",
                        principalColumn: "CalisanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaglikVerileri_CalisanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "SaglikVerileri",
                column: "CalisanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaglikVerileri",
                schema: "gayemkaratas_OrendaAdmin");
        }
    }
}
