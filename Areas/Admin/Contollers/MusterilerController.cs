using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area yapılandırması.
    [Area("Admin")]
    // [İster 25]: Yetkilendirme kontrolü (Sadece yönetici girişi yapanlar erişebilir).
    [Authorize]
    public class MusterilerController : Controller
    {
        private readonly RestoranContext _context;

        // [İster 17]: Veritabanı bağlamı Constructor Injection ile alındı.
        public MusterilerController(RestoranContext context)
        {
            _context = context;
        }

        // [İster 4]: Müşteri verilerinin listelenmesi (Read).
        public IActionResult Index()
        {
            // En son eklenen müşteri en üstte olacak şekilde sıralama
            var musteriler = _context.Musteriler.OrderByDescending(x => x.MusteriID).ToList();
            return View(musteriler);
        }

        // [İster 4 ve 10]: Veri tutarlılığını sağlamak için geçmiş rezervasyonlardan müşteri senkronizasyonu.
        public IActionResult SenkronizeEt()
        {
            var rezervasyonlar = _context.Rezervasyonlar.ToList();
            int eklenenSayisi = 0;

            foreach (var rez in rezervasyonlar)
            {
                // Müşteri kaydı mükerrer olmasın diye E-posta veya Telefon kontrolü yapılıyor.
                var musteriVarMi = _context.Musteriler.Any(m => m.MusteriEposta == rez.MusteriEposta || m.MusteriTelefon == rez.MusteriTelefon);

                if (!musteriVarMi)
                {
                    var yeniMusteri = new Musteri
                    {
                        MusteriAdi = rez.MusteriAdi,
                        MusteriSoyadi = rez.MusteriSoyadi,
                        MusteriEposta = rez.MusteriEposta,
                        MusteriTelefon = rez.MusteriTelefon,
                        // Zorunlu alan (Şifre) için varsayılan değer ataması
                        MusteriSifre = rez.MusteriTelefon
                    };
                    _context.Musteriler.Add(yeniMusteri);
                    eklenenSayisi++;
                }
            }

            if (eklenenSayisi > 0)
            {
                _context.SaveChanges();
                TempData["Basarili"] = $"{eklenenSayisi} adet geçmiş müşteri kaydı başarıyla oluşturuldu.";
            }
            else
            {
                TempData["Bilgi"] = "Senkronize edilecek yeni müşteri bulunamadı. Veriler güncel.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Duzenle(int? id)
        {
            if (id == null) return NotFound();
            var musteri = _context.Musteriler.Find(id);
            if (musteri == null) return NotFound();
            return View(musteri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Müşteri bilgilerini güncelleme işlemi (Update).
        public IActionResult Duzenle(Musteri musteri)
        {
            // [İster 15]: Model doğrulama kontrolü.
            if (ModelState.IsValid)
            {
                _context.Musteriler.Update(musteri);
                _context.SaveChanges();
                TempData["Basarili"] = "Müşteri bilgileri güncellendi.";
                return RedirectToAction("Index");
            }
            return View(musteri);
        }

        // [İster 4]: Müşteri silme işlemi (Delete).
        public IActionResult Sil(int id)
        {
            var musteri = _context.Musteriler.Find(id);
            if (musteri != null)
            {
                _context.Musteriler.Remove(musteri);
                _context.SaveChanges();
                TempData["Basarili"] = "Müşteri silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}