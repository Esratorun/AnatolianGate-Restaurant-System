using System.ComponentModel.DataAnnotations;

namespace RestoranProje1.Models
{
    public class Yonetici
    {
        [Key]
        public int YoneticiID { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)]
        public string YoneticiAdi { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50)]
        public string YoneticiSoyadi { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50)]
        public string YoneticiKullaniciAdi { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Şifre en az 5 karakter olmalıdır.")]
        public string YoneticiSifre { get; set; }
    }
}