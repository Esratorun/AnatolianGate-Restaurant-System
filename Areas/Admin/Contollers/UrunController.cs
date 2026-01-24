using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestoranProje1.Models;
using RestoranProje1.Services;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area yapılandırması.
    [Area("Admin")]
    // [İster 25]: Yetkilendirme kontrolü (Sadece yönetici girişi yapanlar erişebilir).
    [Authorize]
    public class UrunController : Controller
    {
        private readonly RestoranContext _context;
        private readonly IResimService _resimService;           // Transient Servis
        private readonly IIstatistikService _istatistikService; // Singleton Servis

        // [İster 17]: Dependency Injection ile gerekli servislerin ve veritabanı bağlamının enjekte edilmesi.
        public UrunController(RestoranContext context, IResimService resimService, IIstatistikService istatistikService)
        {
            _context = context;
            _resimService = resimService;
            _istatistikService = istatistikService;
        }

        // [İster 4]: Ürünlerin kategori bilgisiyle (Include) listelenmesi (Read).
        // [İster 4]: Ürünlerin kategori bilgisiyle listelenmesi (Read).
        // [YENİ İSTER]: Server-side Paging (Sunucu Taraflı Sayfalama) eklendi.
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            // Toplam kayıt sayısını al
            var toplamUrunSayisi = _context.Urunler.Count();

            // Veritabanından sadece ilgili aralığı çek
            var urunler = _context.Urunler
                .Include(u => u.Kategori)
                .OrderBy(u => u.UrunID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // View tarafına gerekli bilgileri gönder
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)toplamUrunSayisi / pageSize);

            return View(urunler);
        }
        public IActionResult Ekle()
        {
            ViewBag.KategoriID = new SelectList(_context.Kategoriler, "KategoriID", "KategoriAdi");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Yeni ürün ekleme işlemi (Create) - Asenkron.
        public async Task<IActionResult> Ekle(Urun urun, IFormFile? ResimDosyasi)
        {
            // Resim ve Kategori nesnesi validasyon dışı bırakıldı.
            ModelState.Remove("UrunResimYolu");
            ModelState.Remove("Kategori");

            // [İster 15]: Server-side validation.
            if (!ModelState.IsValid)
            {
                ViewBag.KategoriID = new SelectList(_context.Kategoriler, "KategoriID", "KategoriAdi", urun.KategoriID);
                return View(urun);
            }

            // Resim yükleme işlemleri için Transient Servis kullanımı.
            if (ResimDosyasi != null)
            {
                urun.UrunResimYolu = await _resimService.ResimYukleAsync(ResimDosyasi, "urunler");
            }
            else
            {
                urun.UrunResimYolu = "/img/default-food.jpg";
            }

            _context.Urunler.Add(urun);
            await _context.SaveChangesAsync();

            // Singleton servis kullanımı: Sistem genelindeki işlem sayacını arttırır.
            _istatistikService.SayacArttir();

            // [İster 21]: SweetAlert bildirimi için TempData.
            TempData["Basarili"] = "Ürün başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int? id)
        {
            if (id == null) return NotFound();
            var urun = _context.Urunler.Find(id);
            if (urun == null) return NotFound();
            ViewBag.KategoriID = new SelectList(_context.Kategoriler, "KategoriID", "KategoriAdi", urun.KategoriID);
            return View(urun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Ürün güncelleme işlemi (Update) - Asenkron.
        public async Task<IActionResult> Duzenle(int id, Urun urun, IFormFile? ResimDosyasi)
        {
            if (id != urun.UrunID) return NotFound();
            ModelState.Remove("UrunResimYolu");
            ModelState.Remove("Kategori");

            if (ModelState.IsValid)
            {
                try
                {
                    if (ResimDosyasi != null)
                    {
                        // Eski resmin silinmesi ve yenisinin yüklenmesi (Transient Servis).
                        _resimService.ResimSil(urun.UrunResimYolu);
                        urun.UrunResimYolu = await _resimService.ResimYukleAsync(ResimDosyasi, "urunler");
                    }
                    else
                    {
                        // Resim değişmediyse veritabanındaki mevcut yol korunur (AsNoTracking ile performans artışı).
                        var eskiUrun = await _context.Urunler.AsNoTracking().FirstOrDefaultAsync(x => x.UrunID == id);
                        urun.UrunResimYolu = eskiUrun.UrunResimYolu;
                    }

                    _context.Update(urun);
                    await _context.SaveChangesAsync();

                    // Singleton sayaç güncellemesi.
                    _istatistikService.SayacArttir();

                    TempData["Basarili"] = "Ürün güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Urunler.Any(e => e.UrunID == urun.UrunID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.KategoriID = new SelectList(_context.Kategoriler, "KategoriID", "KategoriAdi", urun.KategoriID);
            return View(urun);
        }

        // [İster 4]: Ürün silme işlemi (Delete).
        public IActionResult Sil(int id)
        {
            var urun = _context.Urunler.Find(id);
            if (urun != null)
            {
                // İlişkili resim dosyasının sunucudan temizlenmesi.
                _resimService.ResimSil(urun.UrunResimYolu);

                _context.Urunler.Remove(urun);
                _context.SaveChanges();

                // Singleton sayaç güncellemesi.
                _istatistikService.SayacArttir();

                TempData["Basarili"] = "Ürün silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}