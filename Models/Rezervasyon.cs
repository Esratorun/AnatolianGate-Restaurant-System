using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoranProje1.Models
{
    public class Rezervasyon
    {
        [Key]
        public int RezervasyonID { get; set; }

        // --- Müşteri Bilgileri ---
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)]
        public string MusteriAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50)]
        public string MusteriSoyadi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress]
        public string MusteriEposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon alanı zorunludur.")]
        [Phone]
        public string MusteriTelefon { get; set; } = string.Empty;

        // Rezervasyon Detayları 
        [Required]
        public DateTime RezervasyonTarihi { get; set; }

        [Required]
        public string RezervasyonSaati { get; set; } = string.Empty;

        [Required]
        public int KisiSayisi { get; set; }

        public string? MusteriNotu { get; set; }

        public string RezervasyonDurumu { get; set; } = "Bekliyor";


        public int? MasaID { get; set; }

        [ForeignKey("MasaID")]
        public virtual Masa? Masa { get; set; } 
        //Müşteri İlişkisi
        public int? MusteriID { get; set; }

        [ForeignKey("MusteriID")]
        public virtual Musteri? Musteri { get; set; }
    }
}