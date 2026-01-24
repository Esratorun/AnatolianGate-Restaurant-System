namespace RestoranProje1.Models
{
    // Bu sınıf VERİTABANI tablosu değil, FORM verisi taşıyıcısıdır.
    //Tag helperların çalışması için gerekli
    public class RezervasyonViewModel
    {
        public string AdSoyad { get; set; }
        public string Eposta { get; set; }
        public string Telefon { get; set; }
        public DateTime Tarih { get; set; }
        public string Saat { get; set; }
        public int KisiSayisi { get; set; }
    }
}