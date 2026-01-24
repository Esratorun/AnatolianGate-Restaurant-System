using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoranProje1.Models;
using System.Diagnostics;

namespace RestoranProje1.Controllers
{
    public class HomeController : Controller
    {
        private readonly RestoranContext _context;

        // [Ýster 17]: Dependency Injection (DI)
        // Veritabaný baðlamý constructor üzerinden enjekte ediliyor.
        public HomeController(RestoranContext context)
        {
            _context = context;
        }

        // 1. ANA SAYFA (GET)
        public IActionResult Index()
        {
            // [Ýster 10]: LINQ ve Ýliþkisel Veri Çekme (Include)
            // Kategorileri, içindeki ürünlerle beraber getirir.
            var kategoriler = _context.Kategoriler
                                      .Include(k => k.Urunler)
                                      .ToList();

            var model = new HomeViewModel
            {
                Kategoriler = kategoriler,
                Rezervasyon = new Rezervasyon()
                {
                    RezervasyonTarihi = DateTime.Today,
                    KisiSayisi = 2
                }
            };

            return View(model);
        }

        // 2. MENÜ SAYFASI (GET)
        public IActionResult Menu()
        {
            // [Ýster 4]: Veritabanýndan veri okuma (Read)
            var kategoriler = _context.Kategoriler
                                      .Include(k => k.Urunler)
                                      .ToList();
            return View(kategoriler);
        }

        // 3. AÞÇILAR SAYFASI (GET)
        public IActionResult Ascilar()
        {
            var ascilar = _context.Ascilar.ToList();
            return View(ascilar);
        }

        // 4. HÝKAYEMÝZ SAYFASI (GET)
        public IActionResult Hikayemiz()
        {
            return View();
        }

        // 5. ÝLETÝÞÝM SAYFASI (GET)
        public IActionResult Iletisim()
        {
            return View();
        }

        // 6. REZERVASYON SAYFASI (GET - Ayrý Sayfa)
        public IActionResult Rezervasyon()
        {
            var model = new HomeViewModel
            {
                Rezervasyon = new Rezervasyon()
                {
                    RezervasyonTarihi = DateTime.Today,
                    KisiSayisi = 2
                }
            };
            return View(model);
        }

        // 7. REZERVASYON ÝÞLEMÝ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken] // [Ýster 25]: Güvenlik önlemi (CSRF Korumasý)
        public IActionResult RezervasyonYap(HomeViewModel model)
        {
            // --- VALIDATION TEMÝZLÝÐÝ ---
            // Sadece rezervasyon bilgilerini kontrol etmek için diðer alanlarý validasyondan çýkarýyoruz.
            ModelState.Remove("Kategoriler");
            ModelState.Remove("Rezervasyon.RezervasyonDurumu");
            ModelState.Remove("Rezervasyon.Masa");
            ModelState.Remove("Rezervasyon.Musteri");

            // --- KONTROLLER ---

            // 1. KONTROL: Geçmiþ tarih
            if (model.Rezervasyon.RezervasyonTarihi < DateTime.Today)
            {
                TempData["Hata"] = "Geçmiþ bir tarihe rezervasyon oluþturamazsýnýz.";
                return RedirectToAction("Index", "Home", new { fragment = "reservation" });
            }

            // 2. KONTROL: Kapasite Kontrolü (LINQ ile hesaplama)
            int restoranKapasitesi = 50;

            // [Ýster 10]: LINQ ile karmaþýk sorgulama (Where, Sum)
            var mevcutYogunluk = _context.Rezervasyonlar
                .Where(x => x.RezervasyonTarihi == model.Rezervasyon.RezervasyonTarihi
                         && x.RezervasyonSaati == model.Rezervasyon.RezervasyonSaati
                         && x.RezervasyonDurumu != "Ýptal"
                         && x.RezervasyonDurumu != "Reddedildi")
                .Sum(x => (int?)x.KisiSayisi) ?? 0;

            if ((mevcutYogunluk + model.Rezervasyon.KisiSayisi) > restoranKapasitesi)
            {
                TempData["Hata"] = $"Üzgünüz, seçtiðiniz saat ({model.Rezervasyon.RezervasyonSaati}) için kapasitemiz doludur. Lütfen farklý bir saat seçiniz.";
                return RedirectToAction("Index", "Home", new { fragment = "reservation" });
            }

            // --- KAYIT ÝÞLEMÝ ---
            // [Ýster 15]: Server Side Validation
            if (ModelState.IsValid)
            {
                // ============================================================
                // OTOMATÝK MÜÞTERÝ KAYDI / GÜNCELLEME ALGORÝTMASI
                // ============================================================

                // Müþteriyi E-Posta adresine göre arýyoruz
                var musteri = _context.Musteriler.FirstOrDefault(x => x.MusteriEposta == model.Rezervasyon.MusteriEposta);

                if (musteri == null)
                {
                    // Müþteri yoksa YENÝ KAYIT oluþturuyoruz (Create)
                    var yeniMusteri = new Musteri
                    {
                        MusteriAdi = model.Rezervasyon.MusteriAdi,
                        MusteriSoyadi = model.Rezervasyon.MusteriSoyadi,
                        MusteriEposta = model.Rezervasyon.MusteriEposta,
                        MusteriTelefon = model.Rezervasyon.MusteriTelefon,
                        // Þifre alaný zorunlu olduðu için telefon numarasýný atýyoruz
                        MusteriSifre = model.Rezervasyon.MusteriTelefon
                    };

                    _context.Musteriler.Add(yeniMusteri);
                    _context.SaveChanges();
                }
                else
                {
                    // Müþteri varsa bilgilerini GÜNCELLÝYORUZ (Update)
                    musteri.MusteriAdi = model.Rezervasyon.MusteriAdi;
                    musteri.MusteriSoyadi = model.Rezervasyon.MusteriSoyadi;
                    musteri.MusteriTelefon = model.Rezervasyon.MusteriTelefon;

                    _context.Musteriler.Update(musteri);
                    _context.SaveChanges();
                }
                // ============================================================

                // Rezervasyonu Kaydet
                model.Rezervasyon.RezervasyonDurumu = "Bekliyor";

                // [Ýster 4]: Veritabanýna yeni kayýt ekleme (Create)
                _context.Rezervasyonlar.Add(model.Rezervasyon);
                _context.SaveChanges();

                TempData["Mesaj"] = "Rezervasyon talebiniz baþarýyla alýndý. En kýsa sürede dönüþ yapýlacaktýr.";
                return RedirectToAction("Index");
            }

            // Validasyon hatasý varsa (sayfa yenilenirse) kategorileri tekrar doldur
            model.Kategoriler = _context.Kategoriler.Include(k => k.Urunler).ToList();
            return View("Index", model);
        }

        // Hata Sayfasý
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ÝLETÝÞÝM MESAJI GÖNDERME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IletisimGonder(IletisimMesaj mesaj)
        {
            if (ModelState.IsValid)
            {
                mesaj.GonderimTarihi = DateTime.Now;
                mesaj.OkunduMu = false;

                _context.IletisimMesajlari.Add(mesaj);
                _context.SaveChanges();

                TempData["Mesaj"] = "Mesajýnýz baþarýyla iletildi. Teþekkür ederiz!";
                return RedirectToAction("Iletisim");
            }

            return View("Iletisim", mesaj);
        }

        // 8. REZERVASYON SORGULAMA SAYFASI (GET)
        public IActionResult RezervasyonSorgula()
        {
            return View();
        }

        // 9. REZERVASYON SONUÇLARINI GETÝR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RezervasyonSorgula(string eposta)
        {
            if (string.IsNullOrEmpty(eposta))
            {
                TempData["Hata"] = "Lütfen bir e-posta adresi giriniz.";
                return View();
            }

            // [Ýster 10]: LINQ ile filtreleme ve sýralama
            var rezervasyonlar = _context.Rezervasyonlar
                                         .Where(x => x.MusteriEposta == eposta)
                                         .OrderByDescending(x => x.RezervasyonTarihi)
                                         .ToList();

            if (rezervasyonlar.Count == 0)
            {
                TempData["Hata"] = "Bu e-posta adresiyle kayýtlý rezervasyon bulunamadý.";
                return View();
            }

            return View("RezervasyonSonuc", rezervasyonlar);
        }
    }
}