using System.ComponentModel.DataAnnotations;

namespace RestoranProje1.Models
{
    public class Asci
    {
        [Key]
        public int AsciID { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)]
        public string AsciAdi { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50)]
        public string AsciSoyadi { get; set; }

        [StringLength(50)]
        public string AsciUnvan { get; set; } 

        public string AsciBiyografi { get; set; }

        public string AsciResimYolu { get; set; } 
    }
}