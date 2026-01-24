using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RestoranProje1.Services
{
    // [İster 8]  : Interface (Arayüz) Kullanımı
    // [İster 17] : Dependency Injection için soyutlama katmanı
    public interface IResimService
    {
        // [İster 26] : Asenkron Programlama (Task yapısı)
        Task<string> ResimYukleAsync(IFormFile resimDosyasi, string klasorAdi);

        void ResimSil(string resimYolu);
    }
}