using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoranProje1.Migrations
{
    /// <inheritdoc />
    public partial class MasaIdBosGecilebilir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervasyonlar_Masalar_MasaID",
                table: "Rezervasyonlar");

            migrationBuilder.AlterColumn<int>(
                name: "MasaID",
                table: "Rezervasyonlar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervasyonlar_Masalar_MasaID",
                table: "Rezervasyonlar",
                column: "MasaID",
                principalTable: "Masalar",
                principalColumn: "MasaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rezervasyonlar_Masalar_MasaID",
                table: "Rezervasyonlar");

            migrationBuilder.AlterColumn<int>(
                name: "MasaID",
                table: "Rezervasyonlar",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rezervasyonlar_Masalar_MasaID",
                table: "Rezervasyonlar",
                column: "MasaID",
                principalTable: "Masalar",
                principalColumn: "MasaID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
