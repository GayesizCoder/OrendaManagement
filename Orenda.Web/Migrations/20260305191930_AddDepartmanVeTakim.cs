using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmanVeTakim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AktiflikDurumu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HaftalikVerimlilikSkoru",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departmanlar",
                schema: "gayemkaratas_OrendaAdmin",
                columns: table => new
                {
                    DepartmanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departmanlar", x => x.DepartmanID);
                });

            migrationBuilder.CreateTable(
                name: "Takimlar",
                schema: "gayemkaratas_OrendaAdmin",
                columns: table => new
                {
                    TakimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmanID = table.Column<int>(type: "int", nullable: true),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Takimlar", x => x.TakimID);
                    table.ForeignKey(
                        name: "FK_Takimlar_Departmanlar_DepartmanID",
                        column: x => x.DepartmanID,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "Departmanlar",
                        principalColumn: "DepartmanID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                column: "DepartmanID");

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                column: "TakimID");

            migrationBuilder.CreateIndex(
                name: "IX_Takimlar_DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Takimlar",
                column: "DepartmanID");

            migrationBuilder.AddForeignKey(
                name: "FK_Kisiler_Departmanlar_DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                column: "DepartmanID",
                principalSchema: "gayemkaratas_OrendaAdmin",
                principalTable: "Departmanlar",
                principalColumn: "DepartmanID");

            migrationBuilder.AddForeignKey(
                name: "FK_Kisiler_Takimlar_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler",
                column: "TakimID",
                principalSchema: "gayemkaratas_OrendaAdmin",
                principalTable: "Takimlar",
                principalColumn: "TakimID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kisiler_Departmanlar_DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropForeignKey(
                name: "FK_Kisiler_Takimlar_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropTable(
                name: "Takimlar",
                schema: "gayemkaratas_OrendaAdmin");

            migrationBuilder.DropTable(
                name: "Departmanlar",
                schema: "gayemkaratas_OrendaAdmin");

            migrationBuilder.DropIndex(
                name: "IX_Kisiler_DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropIndex(
                name: "IX_Kisiler_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropColumn(
                name: "AktiflikDurumu",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropColumn(
                name: "DepartmanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropColumn(
                name: "HaftalikVerimlilikSkoru",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");

            migrationBuilder.DropColumn(
                name: "TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "Kisiler");
        }
    }
}
