using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoranProje1.Models;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area'sı oluşturuldu.
    [Area("Admin")]
    // [İster 25]: Yetkisiz erişimi engellemek için Authorization mekanizması uygulandı.
    [Authorize]
    public class AscilarController : Controller
    {
        private readonly RestoranContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        // [İster 17]: Veritabanı context'i ve dosya işlemleri için Dependency Injection kullanıldı.
        public AscilarController(RestoranContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // [İster 4]: Entity Framework kullanılarak verilerin listelenmesi (Read).
        public IActionResult Index()
        {
            var ascilar = _context.Ascilar.ToList();
            return View(ascilar);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Veritabanına yeni kayıt ekleme işlemi (Create).
        public async Task<IActionResult> Ekle(Asci asci, IFormFile? ResimDosyasi)
        {
            // Resim yolu validation dışı bırakıldı, manuel yönetilecek.
            ModelState.Remove("AsciResimYolu");

            // [İster 15]: Server-side validation kontrolü yapıldı.
            if (ModelState.IsValid)
            {
                // Resim yükleme işlemleri
                if (ResimDosyasi != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResimDosyasi.FileName);
                    string uploadPath = Path.Combine(wwwRootPath, "img", "ascilar");

                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                    {
                        await ResimDosyasi.CopyToAsync(stream);
                    }

                    asci.AsciResimYolu = @"/img/ascilar/" + fileName;
                }
                else
                {
                    asci.AsciResimYolu = "/img/default-chef.jpg";
                }

                _context.Ascilar.Add(asci);
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Aşçı başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(asci);
        }

        public IActionResult Duzenle(int? id)
        {
            if (id == null) return NotFound();
            var asci = _context.Ascilar.Find(id);
            if (asci == null) return NotFound();
            return View(asci);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Mevcut kaydın güncellenmesi işlemi (Update).
        public async Task<IActionResult> Duzenle(int id, Asci asci, IFormFile? ResimDosyasi)
        {
            if (id != asci.AsciID) return NotFound();

            ModelState.Remove("AsciResimYolu");

            // [İster 15]: Güncelleme işleminde model doğrulama kontrolü.
            if (ModelState.IsValid)
            {
                if (ResimDosyasi != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResimDosyasi.FileName);
                    string path = Path.Combine(wwwRootPath, "img", "ascilar", fileName);

                    // Eski resim dosyası sunucudan siliniyor
                    if (!string.IsNullOrEmpty(asci.AsciResimYolu) && asci.AsciResimYolu != "/img/default-chef.jpg")
                    {
                        var eskiYol = Path.Combine(wwwRootPath, asci.AsciResimYolu.TrimStart('/'));
                        if (System.IO.File.Exists(eskiYol)) System.IO.File.Delete(eskiYol);
                    }

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ResimDosyasi.CopyToAsync(stream);
                    }
                    asci.AsciResimYolu = @"/img/ascilar/" + fileName;
                }
                else
                {
                    // Resim değişmediyse eski yolu koruyoruz (Change Tracking optimizasyonu)
                    var eskiKayit = _context.Ascilar.AsNoTracking().FirstOrDefault(x => x.AsciID == id);
                    asci.AsciResimYolu = eskiKayit?.AsciResimYolu;
                }

                _context.Update(asci);
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Aşçı güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(asci);
        }

        // [İster 4]: Kayıt silme işlemi (Delete).
        public IActionResult Sil(int id)
        {
            var asci = _context.Ascilar.Find(id);
            if (asci != null)
            {
                // Sunucudaki ilgili resim dosyasının temizlenmesi
                if (!string.IsNullOrEmpty(asci.AsciResimYolu) && asci.AsciResimYolu != "/img/default-chef.jpg")
                {
                    var yol = Path.Combine(_hostEnvironment.WebRootPath, asci.AsciResimYolu.TrimStart('/'));
                    if (System.IO.File.Exists(yol)) System.IO.File.Delete(yol);
                }
                _context.Ascilar.Remove(asci);
                _context.SaveChanges();
                TempData["Basarili"] = "Aşçı silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}