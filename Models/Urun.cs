using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestoranProje1.Models
{
    public class Urun
    {
        [Key]
        public int UrunID { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100)]
        public string UrunAdi { get; set; } 

        public string UrunAciklama { get; set; } 

        [Required]
        public decimal UrunFiyat { get; set; } 

        public string UrunResimYolu { get; set; } 

        public bool UrunOneCikanlar { get; set; } 

        public int KategoriID { get; set; }

        [ForeignKey("KategoriID")]
        public virtual Kategori Kategori { get; set; }
    }
}