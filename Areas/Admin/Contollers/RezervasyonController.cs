using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestoranProje1.Models;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area yapılandırması.
    [Area("Admin")]
    // [İster 25]: Yetkilendirme kontrolü (Sadece yönetici girişi yapanlar erişebilir).
    [Authorize]
    public class RezervasyonController : Controller
    {
        private readonly RestoranContext _context;

        // [İster 17]: Veritabanı bağlamı Constructor Injection ile alındı.
        public RezervasyonController(RestoranContext context)
        {
            _context = context;
        }

        // [İster 4]: Rezervasyonların ilişkili masa verileriyle birlikte listelenmesi (Read).
        public IActionResult Index()
        {
            var rezervasyonlar = _context.Rezervasyonlar
                                         .Include(r => r.Masa)
                                         .OrderByDescending(r => r.RezervasyonTarihi)
                                         .ToList();
            return View(rezervasyonlar);
        }

        public IActionResult Ekle()
        {
            MasalariDoldur();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Yeni rezervasyon oluşturma işlemi (Create).
        public IActionResult Ekle(Rezervasyon rezervasyon)
        {
            // İlişkisel alanlar validasyon dışı bırakıldı (Manuel kontrol edilecek).
            ModelState.Remove("Masa");
            ModelState.Remove("Musteri");
            ModelState.Remove("RezervasyonDurumu");

            // [İster 15]: İş Kuralı 1 - Restoran genel kapasite kontrolü.
            if (!KapasiteUygunMu(rezervasyon.RezervasyonTarihi, rezervasyon.RezervasyonSaati, rezervasyon.KisiSayisi, 0))
            {
                TempData["Hata"] = $"Seçilen saat ({rezervasyon.RezervasyonSaati}) için restoran kapasitesi dolu.";
                MasalariDoldur(rezervasyon.MasaID);
                return View(rezervasyon);
            }

            // [İster 15]: İş Kuralı 2 - Masa çakışma kontrolü (Aynı saatte aynı masa dolu mu?).
            if (!MasaMusaitMi(rezervasyon.MasaID, rezervasyon.RezervasyonTarihi, rezervasyon.RezervasyonSaati, 0))
            {
                var masaAdi = _context.Masalar.Find(rezervasyon.MasaID)?.MasaAdi ?? "Seçilen masa";
                TempData["Hata"] = $"HATA: {masaAdi}, saat {rezervasyon.RezervasyonSaati} için zaten dolu!";
                MasalariDoldur(rezervasyon.MasaID);
                return View(rezervasyon);
            }

            if (ModelState.IsValid)
            {
                // [İster 10]: Rezervasyon yapan kişinin Müşteri tablosuna otomatik senkronizasyonu.
                var musteri = _context.Musteriler.FirstOrDefault(x => x.MusteriEposta == rezervasyon.MusteriEposta);

                if (musteri == null)
                {
                    // Yeni müşteri kaydı oluşturuluyor.
                    var yeniMusteri = new Musteri
                    {
                        MusteriAdi = rezervasyon.MusteriAdi,
                        MusteriSoyadi = rezervasyon.MusteriSoyadi,
                        MusteriEposta = rezervasyon.MusteriEposta,
                        MusteriTelefon = rezervasyon.MusteriTelefon,
                        MusteriSifre = rezervasyon.MusteriTelefon // Varsayılan şifre ataması
                    };
                    _context.Musteriler.Add(yeniMusteri);
                    _context.SaveChanges();
                }
                else
                {
                    // Mevcut müşteri bilgileri güncelleniyor.
                    musteri.MusteriAdi = rezervasyon.MusteriAdi;
                    musteri.MusteriSoyadi = rezervasyon.MusteriSoyadi;
                    musteri.MusteriTelefon = rezervasyon.MusteriTelefon;
                    _context.Musteriler.Update(musteri);
                    _context.SaveChanges();
                }

                rezervasyon.RezervasyonDurumu = "Onaylandı";
                _context.Rezervasyonlar.Add(rezervasyon);
                _context.SaveChanges();
                TempData["Basarili"] = "Rezervasyon başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            MasalariDoldur(rezervasyon.MasaID);
            return View(rezervasyon);
        }

        public IActionResult Duzenle(int? id)
        {
            if (id == null) return NotFound();
            var rez = _context.Rezervasyonlar.Find(id);
            if (rez == null) return NotFound();

            MasalariDoldur(rez.MasaID);
            return View(rez);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [İster 4]: Rezervasyon güncelleme işlemi (Update).
        public IActionResult Duzenle(Rezervasyon rezervasyon)
        {
            ModelState.Remove("Masa");
            ModelState.Remove("Musteri");

            // Kapasite ve Masa Çakışma Kontrolleri (Düzenleme modu için ID hariç tutulur)
            if (!KapasiteUygunMu(rezervasyon.RezervasyonTarihi, rezervasyon.RezervasyonSaati, rezervasyon.KisiSayisi, rezervasyon.RezervasyonID))
            {
                TempData["Hata"] = "Kapasite dolu! Bu değişiklik yapılamaz.";
                MasalariDoldur(rezervasyon.MasaID);
                return View(rezervasyon);
            }

            if (!MasaMusaitMi(rezervasyon.MasaID, rezervasyon.RezervasyonTarihi, rezervasyon.RezervasyonSaati, rezervasyon.RezervasyonID))
            {
                var masaAdi = _context.Masalar.Find(rezervasyon.MasaID)?.MasaAdi ?? "Seçilen masa";
                TempData["Hata"] = $"HATA: {masaAdi}, saat {rezervasyon.RezervasyonSaati} için başka bir müşteriye ayrılmış!";
                MasalariDoldur(rezervasyon.MasaID);
                return View(rezervasyon);
            }

            if (ModelState.IsValid)
            {
                // Müşteri bilgisi de güncelleniyor.
                var musteri = _context.Musteriler.FirstOrDefault(x => x.MusteriEposta == rezervasyon.MusteriEposta);
                if (musteri != null)
                {
                    musteri.MusteriAdi = rezervasyon.MusteriAdi;
                    musteri.MusteriSoyadi = rezervasyon.MusteriSoyadi;
                    musteri.MusteriTelefon = rezervasyon.MusteriTelefon;
                    _context.Musteriler.Update(musteri);
                }

                _context.Rezervasyonlar.Update(rezervasyon);
                _context.SaveChanges();
                TempData["Basarili"] = "Rezervasyon güncellendi.";
                return RedirectToAction("Index");
            }

            MasalariDoldur(rezervasyon.MasaID);
            return View(rezervasyon);
        }

        public IActionResult Onayla(int id)
        {
            var rez = _context.Rezervasyonlar.Find(id);
            if (rez != null) { rez.RezervasyonDurumu = "Onaylandı"; _context.SaveChanges(); TempData["Basarili"] = "Onaylandı."; }
            return RedirectToAction("Index");
        }

        public IActionResult Iptal(int id)
        {
            var rez = _context.Rezervasyonlar.Find(id);
            if (rez != null) { rez.RezervasyonDurumu = "İptal"; rez.MasaID = null; _context.SaveChanges(); TempData["Hata"] = "İptal edildi."; }
            return RedirectToAction("Index");
        }

        // [İster 4]: Rezervasyon silme işlemi (Delete).
        public IActionResult Sil(int id)
        {
            var rez = _context.Rezervasyonlar.Find(id);
            if (rez != null) { _context.Rezervasyonlar.Remove(rez); _context.SaveChanges(); TempData["Basarili"] = "Silindi."; }
            return RedirectToAction("Index");
        }

        // --- YARDIMCI METOTLAR ---

        private void MasalariDoldur(int? seciliMasaId = null)
        {
            // Sadece aktif masaların listelenmesi.
            var masalar = _context.Masalar.Where(x => x.MasaAktifMi == true).ToList();
            ViewBag.Masalar = new SelectList(masalar, "MasaID", "MasaAdi", seciliMasaId);
        }

        // Kapasite kontrol mantığı
        private bool KapasiteUygunMu(DateTime tarih, string saat, int yeniKisiSayisi, int haricTutulacakID)
        {
            int toplamKapasite = 50;
            var doluKoltukSayisi = _context.Rezervasyonlar
                .Where(x => x.RezervasyonTarihi == tarih
                         && x.RezervasyonSaati == saat
                         && x.RezervasyonDurumu != "İptal"
                         && x.RezervasyonDurumu != "Reddedildi"
                         && x.RezervasyonID != haricTutulacakID)
                .Sum(x => (int?)x.KisiSayisi) ?? 0;
            return (doluKoltukSayisi + yeniKisiSayisi) <= toplamKapasite;
        }

        // Masa müsaitlik kontrol mantığı
        private bool MasaMusaitMi(int? masaId, DateTime tarih, string saat, int haricTutulacakID)
        {
            if (masaId == null) return true;

            var cakisma = _context.Rezervasyonlar.Any(x =>
                x.MasaID == masaId &&
                x.RezervasyonTarihi == tarih &&
                x.RezervasyonSaati == saat &&
                x.RezervasyonDurumu != "İptal" &&
                x.RezervasyonDurumu != "Reddedildi" &&
                x.RezervasyonID != haricTutulacakID
            );

            return !cakisma;
        }

        // [İster 19]: AJAX çağrısı için JSON veri döndüren metot.
        [HttpGet]
        public JsonResult GetMusaitlikDurumu(string tarih, string saat)
        {
            int toplamKapasite = 50;
            if (string.IsNullOrEmpty(tarih) || string.IsNullOrEmpty(saat)) return Json(new { kalan = 0 });
            DateTime secilenTarih;
            if (!DateTime.TryParse(tarih, out secilenTarih)) return Json(new { kalan = 0 });
            var dolu = _context.Rezervasyonlar.Where(x => x.RezervasyonTarihi == secilenTarih && x.RezervasyonSaati == saat && x.RezervasyonDurumu != "İptal" && x.RezervasyonDurumu != "Reddedildi").Sum(x => (int?)x.KisiSayisi) ?? 0;
            return Json(new { kalan = toplamKapasite - dolu, toplam = toplamKapasite, doluluk = dolu });
        }

        // [İster 19]: AJAX ile dinamik masa filtreleme servisi.
        [HttpGet]
        public JsonResult GetMusaitMasalar(string tarih, string saat, int? haricRezervasyonId)
        {
            if (string.IsNullOrEmpty(tarih) || string.IsNullOrEmpty(saat))
            {
                return Json(new List<object>());
            }

            DateTime secilenTarih;
            if (!DateTime.TryParse(tarih, out secilenTarih))
            {
                return Json(new List<object>());
            }

            // Dolu masaların tespiti
            var doluMasaIdleri = _context.Rezervasyonlar
                .Where(x => x.RezervasyonTarihi == secilenTarih
                         && x.RezervasyonSaati == saat
                         && x.RezervasyonDurumu != "İptal"
                         && x.RezervasyonDurumu != "Reddedildi"
                         && (haricRezervasyonId == null || x.RezervasyonID != haricRezervasyonId))
                .Select(x => x.MasaID)
                .ToList();

            // Müsait masaların filtrelenmesi
            var musaitMasalar = _context.Masalar.Where(x => x.MasaAktifMi == true)
                .Where(m => !doluMasaIdleri.Contains(m.MasaID))
                .Select(m => new
                {
                    value = m.MasaID,
                    text = m.MasaAdi
                })
                .ToList();

            return Json(musaitMasalar);
        }
    }
}