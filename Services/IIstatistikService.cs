namespace RestoranProje1.Services
{
    // [İster 8]  : Interface (Arayüz) Tanımlanmış mı? EVET.
    // [İster 17] : Dependency Injection için kontrat oluşturulmuş mu? EVET.
    public interface IIstatistikService
    {
        void SayacArttir(); // İşlem yapıldıkça sayacı 1 arttırır
        int SayacGetir();   // Mevcut sayaç değerini döndürür
    }
}