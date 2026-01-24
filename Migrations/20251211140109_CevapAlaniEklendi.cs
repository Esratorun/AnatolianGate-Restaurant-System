using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoranProje1.Migrations
{
    /// <inheritdoc />
    public partial class CevapAlaniEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminCevabi",
                table: "IletisimMesajlari",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminCevabi",
                table: "IletisimMesajlari");
        }
    }
}
