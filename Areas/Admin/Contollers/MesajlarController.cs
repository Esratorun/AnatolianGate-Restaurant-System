using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli için Admin Area yapılandırması.
    [Area("Admin")]
    // [İster 25]: Yetkilendirme kontrolü (Sadece giriş yapmış yöneticiler erişebilir).
    [Authorize]
    public class MesajlarController : Controller
    {
        private readonly RestoranContext _context;

        // [İster 17]: Veritabanı bağlamı Constructor Injection ile alındı.
        public MesajlarController(RestoranContext context)
        {
            _context = context;
        }

        // [İster 4]: Gelen iletişim mesajlarının listelenmesi (Read).
        public IActionResult Index()
        {
            var mesajlar = _context.IletisimMesajlari.OrderByDescending(m => m.GonderimTarihi).ToList();
            return View(mesajlar);
        }

        // [İster 4]: Mesaj silme işlemi (Delete).
        public IActionResult Sil(int id)
        {
            var mesaj = _context.IletisimMesajlari.Find(id);
            if (mesaj != null)
            {
                _context.IletisimMesajlari.Remove(mesaj);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Cevapla(int id)
        {
            var mesaj = _context.IletisimMesajlari.Find(id);
            if (mesaj == null)
            {
                return NotFound();
            }
            return View(mesaj);
        }

        [HttpPost]
        // [İster 4]: Mesaj cevaplama ve durum güncelleme işlemi (Update).
        public IActionResult Cevapla(IletisimMesaj model)
        {
            var mesaj = _context.IletisimMesajlari.Find(model.MesajID);
            if (mesaj != null)
            {
                mesaj.AdminCevabi = model.AdminCevabi;
                mesaj.OkunduMu = true;
                _context.SaveChanges();

                // [İster 21]: İşlem sonucu kullanıcıya bildirim gösterilmesi (SweetAlert entegrasyonu).
                TempData["Basarili"] = "Cevabınız kaydedildi ve mesaj yanıtlandı olarak işaretlendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}