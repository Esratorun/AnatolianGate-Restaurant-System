using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area'sı tanımlandı.
    [Area("Admin")]
    // [İster 25]: Yetkilendirme (Authorization) mekanizması ile güvenli erişim sağlandı.
    [Authorize]
    public class MasalarController : Controller
    {
        private readonly RestoranContext _context;

        // [İster 17]: Veritabanı context'i Dependency Injection ile enjekte edildi.
        public MasalarController(RestoranContext context)
        {
            _context = context;
        }

        // [İster 4]: Kayıtlı masaların listelenmesi (Read).
        public IActionResult Index()
        {
            var masalar = _context.Masalar.ToList();
            return View(masalar);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Yeni masa kaydı ekleme işlemi (Create).
        public IActionResult Ekle(Masa masa)
        {
            // İlişkisel alan (Rezervasyonlar) validasyon dışı bırakıldı.
            ModelState.Remove("Rezervasyonlar");

            // [İster 15]: Server-side validation kontrolü.
            if (ModelState.IsValid)
            {
                masa.MasaAktifMi = true;
                _context.Masalar.Add(masa);
                _context.SaveChanges();
                TempData["Basarili"] = "Masa başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(masa);
        }

        public IActionResult Duzenle(int id)
        {
            var masa = _context.Masalar.Find(id);
            if (masa == null)
            {
                return NotFound();
            }
            return View(masa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Mevcut masa bilgilerinin güncellenmesi (Update).
        public IActionResult Duzenle(Masa masa)
        {
            ModelState.Remove("Rezervasyonlar");

            if (ModelState.IsValid)
            {
                _context.Masalar.Update(masa);
                _context.SaveChanges();
                TempData["Basarili"] = "Masa bilgileri güncellendi.";
                return RedirectToAction("Index");
            }
            return View(masa);
        }

        // [İster 4]: Masa silme işlemi (Delete) ve İlişkisel Bütünlük Kontrolü.
        public IActionResult Sil(int id)
        {
            var masa = _context.Masalar.Find(id);
            if (masa != null)
            {
                // İlişkisel veri bütünlüğünü korumak için, silinen masaya ait 
                // rezervasyonların masa bağlantısı koparılıyor (Null yapılıyor).
                // Bu sayede veri kaybı ve foreign key hatası önleniyor.
                var rezervasyonlar = _context.Rezervasyonlar.Where(r => r.MasaID == id).ToList();

                foreach (var rez in rezervasyonlar)
                {
                    rez.MasaID = null;
                }
                _context.SaveChanges();

                _context.Masalar.Remove(masa);
                _context.SaveChanges();

                TempData["Basarili"] = "Masa başarıyla silindi.";
            }
            else
            {
                TempData["Hata"] = "Silinecek masa bulunamadı.";
            }
            return RedirectToAction("Index");
        }

        // Masa durumunu (Aktif/Pasif) güncelleme
        public IActionResult DurumDegistir(int id)
        {
            var masa = _context.Masalar.Find(id);
            if (masa != null)
            {
                masa.MasaAktifMi = !masa.MasaAktifMi;
                _context.SaveChanges();
                TempData["Basarili"] = "Masa durumu değiştirildi.";
            }
            return RedirectToAction("Index");
        }
    }
}