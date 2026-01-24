namespace RestoranProje1.Services
{
    // [İster 17] : Dependency Injection (Bağımlılık Enjeksiyonu)
    // Bu sınıf, IIstatistikService arayüzünü implemente ederek (uygulayarak)
    // projedeki sayaç mantığını çalıştıran somut servistir.
    public class IstatistikService : IIstatistikService
    {
        // Sayacı hafızada tutacak değişken
        private int _toplamIslem = 0;

        // Sayacı 1 arttırır
        public void SayacArttir()
        {
            _toplamIslem++;
        }

        // Mevcut sayıyı döndürür
        public int SayacGetir()
        {
            return _toplamIslem;
        }
    }
}