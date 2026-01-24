using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;
using RestoranProje1.Services;

namespace RestoranProje1.Areas.Admin.Controllers
{
    // [İster 23]: Yönetim paneli modülü için 'Admin' Area'sı tanımlandı.
    [Area("Admin")]
    // [İster 25]: Bu controllera sadece giriş yapmış yetkili kullanıcıların erişmesi sağlandı (Authorization).
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly RestoranContext _context;
        private readonly IIstatistikService _istatistikService;

        // [İster 17]: Veritabanı bağlamı ve İstatistik servisi Constructor Injection ile alındı.
        public DashboardController(RestoranContext context, IIstatistikService istatistikService)
        {
            _context = context;
            _istatistikService = istatistikService;
        }

        // [İster 4]: Admin ana sayfasında gösterilecek özet verilerin (Dashboard) veritabanından çekilmesi.
        public IActionResult Index()
        {
            // LINQ sorguları ile sayısal verilerin hesaplanması
            ViewBag.UrunSayisi = _context.Urunler.Count();
            ViewBag.KategoriSayisi = _context.Kategoriler.Count();
            ViewBag.AsciSayisi = _context.Ascilar.Count();

            // Tarih bazlı filtreleme ile bugünkü rezervasyonlar
            ViewBag.BugunkuRezervasyon = _context.Rezervasyonlar.Where(x => x.RezervasyonTarihi.Date == DateTime.Today).Count();
            ViewBag.ToplamRezervasyon = _context.Rezervasyonlar.Count();

            // Singleton servis üzerinden uygulama yaşam döngüsü boyunca yapılan işlem sayısı
            ViewBag.OturumIslemSayisi = _istatistikService.SayacGetir();

            return View();
        }
    }
}