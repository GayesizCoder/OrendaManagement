using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddGorevAdimi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GorevAdimlari",
                schema: "gayemkaratas_OrendaAdmin",
                columns: table => new
                {
                    AdimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GorevNo = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false),
                    AgirlikYuzdesi = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GorevAdimlari", x => x.AdimID);
                    table.ForeignKey(
                        name: "FK_GorevAdimlari_ToDo_GorevNo",
                        column: x => x.GorevNo,
                        principalSchema: "gayemkaratas_OrendaAdmin",
                        principalTable: "ToDo",
                        principalColumn: "GorevNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GorevAdimlari_GorevNo",
                schema: "gayemkaratas_OrendaAdmin",
                table: "GorevAdimlari",
                column: "GorevNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GorevAdimlari",
                schema: "gayemkaratas_OrendaAdmin");
        }
    }
}
