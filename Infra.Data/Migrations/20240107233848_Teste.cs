using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vaga_AspNetUsers_UserId",
                table: "Vaga");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vaga",
                table: "Vaga");

            migrationBuilder.RenameTable(
                name: "Vaga",
                newName: "Vagas");

            migrationBuilder.RenameIndex(
                name: "IX_Vaga_UserId",
                table: "Vagas",
                newName: "IX_Vagas_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Vagas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Company",
                table: "Vagas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vagas",
                table: "Vagas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vagas_AspNetUsers_UserId",
                table: "Vagas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vagas_AspNetUsers_UserId",
                table: "Vagas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vagas",
                table: "Vagas");

            migrationBuilder.RenameTable(
                name: "Vagas",
                newName: "Vaga");

            migrationBuilder.RenameIndex(
                name: "IX_Vagas_UserId",
                table: "Vaga",
                newName: "IX_Vaga_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Vaga",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Company",
                table: "Vaga",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vaga",
                table: "Vaga",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaga_AspNetUsers_UserId",
                table: "Vaga",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
