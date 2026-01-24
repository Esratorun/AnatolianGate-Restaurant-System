using RestoranProje1.Models;
using System.ComponentModel.DataAnnotations;

namespace RestoranProje1.Models
{
    public class Kategori
    {
        [Key]
        public int KategoriID { get; set; }

        [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
        [StringLength(50)]
        public string KategoriAdi { get; set; }

        public virtual ICollection<Urun>? Urunler { get; set; }
    }
}