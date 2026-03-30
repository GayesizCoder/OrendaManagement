using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orenda.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTakimIDToDo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDo_Kisiler",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");

            migrationBuilder.AlterColumn<int>(
                name: "AtananCalisanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDo_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                column: "TakimID");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDo_Kisiler",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                column: "AtananCalisanID",
                principalSchema: "gayemkaratas_OrendaAdmin",
                principalTable: "Kisiler",
                principalColumn: "CalisanID");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDo_Takimlar_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                column: "TakimID",
                principalSchema: "gayemkaratas_OrendaAdmin",
                principalTable: "Takimlar",
                principalColumn: "TakimID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDo_Kisiler",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");

            migrationBuilder.DropForeignKey(
                name: "FK_ToDo_Takimlar_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");

            migrationBuilder.DropIndex(
                name: "IX_ToDo_TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");

            migrationBuilder.DropColumn(
                name: "TakimID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo");

            migrationBuilder.AlterColumn<int>(
                name: "AtananCalisanID",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDo_Kisiler",
                schema: "gayemkaratas_OrendaAdmin",
                table: "ToDo",
                column: "AtananCalisanID",
                principalSchema: "gayemkaratas_OrendaAdmin",
                principalTable: "Kisiler",
                principalColumn: "CalisanID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
