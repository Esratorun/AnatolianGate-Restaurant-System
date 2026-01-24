using System.ComponentModel.DataAnnotations;

namespace RestoranProje1.Models
{
    public class Masa
    {
        [Key]
        public int MasaID { get; set; } 

        [Required]
        [StringLength(20)]
        public string MasaAdi { get; set; }

        [Required]
        public int MasaKapasite { get; set; } 

        public string MasaKonum { get; set; } 

        public bool MasaAktifMi { get; set; } = true; 

    
        public virtual ICollection<Rezervasyon> Rezervasyonlar { get; set; }
    }
}