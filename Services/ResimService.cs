using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RestoranProje1.Services
{
    // [İster 17] : Dependency Injection - Somut Servis Sınıfı
    public class ResimService : IResimService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        // [İster 17] : Constructor Injection
        public ResimService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        // [İster 26] : Asenkron Dosya Yükleme İşlemi
        public async Task<string> ResimYukleAsync(IFormFile resimDosyasi, string klasorAdi)
        {
            if (resimDosyasi == null) return "/img/default-food.jpg";

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
            string uploadPath = Path.Combine(wwwRootPath, "img", klasorAdi);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
            {
                await resimDosyasi.CopyToAsync(fileStream);
            }

            return @$"/img/{klasorAdi}/" + fileName;
        }

        // Dosya Silme İşlemi
        public void ResimSil(string resimYolu)
        {
            if (!string.IsNullOrEmpty(resimYolu) && !resimYolu.Contains("default"))
            {
                var tamYol = Path.Combine(_hostEnvironment.WebRootPath, resimYolu.TrimStart('/'));

                if (File.Exists(tamYol))
                {
                    File.Delete(tamYol);
                }
            }
        }
    }
}