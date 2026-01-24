using RestoranProje1.Models; 

namespace RestoranProje1.Models
{
    public class HomeViewModel
    {
        // Menü listesi için
        public List<Kategori> Kategoriler { get; set; } = new List<Kategori>();

        // Rezervasyon formu için
        public Rezervasyon Rezervasyon { get; set; } = new Rezervasyon();
    }
}