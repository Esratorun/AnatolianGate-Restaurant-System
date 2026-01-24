using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area yapılandırması.
    [Area("Admin")]
    // [İster 25]: Yetkilendirme kontrolü (Sadece giriş yapmış yöneticiler erişebilir).
    [Authorize]
    public class KategoriController : Controller
    {
        private readonly RestoranContext _context;

        // [İster 17]: Veritabanı bağlamı Constructor Injection ile alındı.
        public KategoriController(RestoranContext context)
        {
            _context = context;
        }

        // [İster 4]: Kategori verilerinin listelenmesi (Read).
        public IActionResult Index()
        {
            var kategoriler = _context.Kategoriler.ToList();
            return View(kategoriler);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı güvenlik önlemi.
        // [İster 4]: Yeni kategori ekleme işlemi (Create).
        public IActionResult Ekle(Kategori kategori)
        {
            // [İster 15]: Sunucu taraflı doğrulama (Server-side validation).
            if (ModelState.IsValid)
            {
                _context.Kategoriler.Add(kategori);
                _context.SaveChanges();

                // [İster 21]: Kullanıcı geri bildirimi için TempData kullanımı (View tarafında SweetAlert tetiklenir).
                TempData["Basarili"] = "Kategori başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(kategori);
        }

        public IActionResult Duzenle(int? id)
        {
            if (id == null) return NotFound();
            var kategori = _context.Kategoriler.Find(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Kategori güncelleme işlemi (Update).
        public IActionResult Duzenle(Kategori kategori)
        {
            // [İster 15]: Model doğrulama kontrolü.
            if (ModelState.IsValid)
            {
                _context.Kategoriler.Update(kategori);
                _context.SaveChanges();
                TempData["Basarili"] = "Kategori güncellendi.";
                return RedirectToAction("Index");
            }
            return View(kategori);
        }

        // [İster 4]: Kategori silme işlemi (Delete).
        public IActionResult Sil(int id)
        {
            var kategori = _context.Kategoriler.Find(id);
            if (kategori != null)
            {
                _context.Kategoriler.Remove(kategori);
                _context.SaveChanges();
                TempData["Basarili"] = "Kategori silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}