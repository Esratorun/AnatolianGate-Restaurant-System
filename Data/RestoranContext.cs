using Microsoft.EntityFrameworkCore;

namespace RestoranProje1.Models
{
    // [İster 2] : Entity Framework (ORM) kullanımı
    // [İster 6] : Code First Yaklaşımı (Sınıftan veritabanına)
    public class RestoranContext : DbContext
    {
        public RestoranContext(DbContextOptions<RestoranContext> options) : base(options)
        {
        }

        // [İster 3] : Veritabanı tablolarının (DbSet) tanımlanması
        public DbSet<Asci> Ascilar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Masa> Masalar { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Yonetici> Yoneticiler { get; set; }
        public DbSet<IletisimMesaj> IletisimMesajlari { get; set; }

        // [İster 10] : Fluent API Kullanımı (OnModelCreating)
        // Veritabanı oluşurken çalışacak özel kurallar burada tanımlanır.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. UrunFiyat Hassasiyet Ayarı
            // Bu ayar yapılmazsa EF Core migration sırasında "decimal precision" uyarısı verir.
            modelBuilder.Entity<Urun>()
                .Property(u => u.UrunFiyat)
                .HasColumnType("decimal(18,2)");

            // 2. Varsayılan Admin Kullanıcısı Ekleme (Data Seeding)
            // Proje ilk açıldığında veritabanına otomatik olarak bu kullanıcıyı ekler.
            // Böylece "Admin" paneline giriş yapabilirsin.
            modelBuilder.Entity<Yonetici>().HasData(
                new Yonetici
                {
                    YoneticiID = 1,
                    YoneticiAdi = "Esra",
                    YoneticiSoyadi = "Torun",
                    YoneticiKullaniciAdi = "admin",
                    YoneticiSifre = "123" // Şifre: 123
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}