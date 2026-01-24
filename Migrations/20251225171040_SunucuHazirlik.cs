using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoranProje1.Migrations
{
    /// <inheritdoc />
    public partial class SunucuHazirlik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Yoneticiler",
                columns: new[] { "YoneticiID", "YoneticiAdi", "YoneticiKullaniciAdi", "YoneticiSifre", "YoneticiSoyadi" },
                values: new object[] { 1, "Esra", "admin", "123", "Torun" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Yoneticiler",
                keyColumn: "YoneticiID",
                keyValue: 1);
        }
    }
}
