using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestoranProje1.Migrations
{
    /// <inheritdoc />
    public partial class Kurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ascilar",
                columns: table => new
                {
                    AsciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AsciSoyadi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AsciUnvan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AsciBiyografi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AsciResimYolu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ascilar", x => x.AsciID);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    KategoriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KategoriAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.KategoriID);
                });

            migrationBuilder.CreateTable(
                name: "Masalar",
                columns: table => new
                {
                    MasaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MasaAdi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MasaKapasite = table.Column<int>(type: "int", nullable: false),
                    MasaKonum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MasaAktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Masalar", x => x.MasaID);
                });

            migrationBuilder.CreateTable(
                name: "Musteriler",
                columns: table => new
                {
                    MusteriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusteriAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MusteriSoyadi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MusteriEposta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MusteriSifre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MusteriTelefon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musteriler", x => x.MusteriID);
                });

            migrationBuilder.CreateTable(
                name: "Yoneticiler",
                columns: table => new
                {
                    YoneticiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YoneticiAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YoneticiSoyadi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YoneticiKullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YoneticiSifre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yoneticiler", x => x.YoneticiID);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    UrunID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UrunAciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrunFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UrunResimYolu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrunOneCikanlar = table.Column<bool>(type: "bit", nullable: false),
                    KategoriID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.UrunID);
                    table.ForeignKey(
                        name: "FK_Urunler_Kategoriler_KategoriID",
                        column: x => x.KategoriID,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervasyonlar",
                columns: table => new
                {
                    RezervasyonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusteriAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MusteriSoyadi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MusteriEposta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MusteriTelefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RezervasyonTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RezervasyonSaati = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KisiSayisi = table.Column<int>(type: "int", nullable: false),
                    MusteriNotu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RezervasyonDurumu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MasaID = table.Column<int>(type: "int", nullable: false),
                    MusteriID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervasyonlar", x => x.RezervasyonID);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Masalar_MasaID",
                        column: x => x.MasaID,
                        principalTable: "Masalar",
                        principalColumn: "MasaID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Musteriler_MusteriID",
                        column: x => x.MusteriID,
                        principalTable: "Musteriler",
                        principalColumn: "MusteriID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_MasaID",
                table: "Rezervasyonlar",
                column: "MasaID");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_MusteriID",
                table: "Rezervasyonlar",
                column: "MusteriID");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_KategoriID",
                table: "Urunler",
                column: "KategoriID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ascilar");

            migrationBuilder.DropTable(
                name: "Rezervasyonlar");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Yoneticiler");

            migrationBuilder.DropTable(
                name: "Masalar");

            migrationBuilder.DropTable(
                name: "Musteriler");

            migrationBuilder.DropTable(
                name: "Kategoriler");
        }
    }
}
