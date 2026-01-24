using System.ComponentModel.DataAnnotations;

namespace RestoranProje1.Models
{
    public class Musteri
    {
        [Key]
        public int MusteriID { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)]
        public string MusteriAdi { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50)]
        public string MusteriSoyadi { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string MusteriEposta { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string MusteriSifre { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone]
        public string MusteriTelefon { get; set; }

        public virtual ICollection<Rezervasyon> Rezervasyonlar { get; set; }
    }
}