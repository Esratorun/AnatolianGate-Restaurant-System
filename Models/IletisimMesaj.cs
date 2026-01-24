using System.ComponentModel.DataAnnotations;

namespace RestoranProje1.Models
{
    public class IletisimMesaj
    {
        [Key]
        public int MesajID { get; set; }

        [Required]
        [StringLength(100)]
        public string AdSoyad { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Eposta { get; set; } = string.Empty;

        public string? Konu { get; set; }

        [Required]
        public string MesajIcerigi { get; set; } = string.Empty;

        public DateTime GonderimTarihi { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;

        public string? AdminCevabi { get; set; } 
    }
}