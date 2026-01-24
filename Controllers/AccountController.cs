using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;
using System.Security.Claims;

namespace RestoranProje1.Controllers
{
    // [İster 9]  : Proje MVC tasarım kalıbına uygun geliştirilmiş mi? EVET.
    public class AccountController : Controller
    {
        private readonly RestoranContext _context;

        // [İster 17] : Dependency Injection (DI) kullanılmış mı?
        //              EVET -> Veritabanı bağlamı (Context) Constructor üzerinden enjekte edilmiştir.
        public AccountController(RestoranContext context)
        {
            _context = context;
        }

        // 1. GİRİŞ SAYFASI (GET)
        [HttpGet]
        public IActionResult Login()
        {
            // Eğer kullanıcı zaten giriş yapmışsa direkt panele gönder
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View();
        }

        // 2. GİRİŞ İŞLEMİ (POST)
        // [İster 24] : Yönetici girişi yapılıyor mu? EVET.
        // [İster 25] : Güvenlik önlemleri (Cookie Auth) alındı mı? EVET.
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı güvenlik önlemi
        public async Task<IActionResult> Login(string kadi, string sifre)
        {
            // Veritabanından yöneticiyi bul
            var yonetici = _context.Yoneticiler.FirstOrDefault(x => x.YoneticiKullaniciAdi == kadi && x.YoneticiSifre == sifre);

            if (yonetici != null)
            {
                // [İster 25] : Kullanıcı kimlik bilgilerinin (Claims) oluşturulması
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, yonetici.YoneticiKullaniciAdi),
                    new Claim("FullName", yonetici.YoneticiAdi + " " + yonetici.YoneticiSoyadi),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Beni hatırla özelliği gibi davranır
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60) // Oturum süresi
                };

                // [İster 25] : Sisteme güvenli giriş yapılması (Cookie oluşturma)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Admin paneline yönlendirme
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        // 3.ŞİFREMİ UNUTTUM SAYFASI (GET)
        [HttpGet]
        public IActionResult SifremiUnuttum()
        {
            return View();
        }

        // 4. ŞİFREMİ UNUTTUM KONTROL (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SifremiUnuttum(string kadi)
        {
            var yonetici = _context.Yoneticiler.FirstOrDefault(x => x.YoneticiKullaniciAdi == kadi);

            if (yonetici == null)
            {
                TempData["Hata"] = "Böyle bir kullanıcı adı bulunamadı.";
                return View();
            }

            // Kullanıcı bulunduysa Şifre Yenileme sayfasına yönlendir
            TempData["SifirlanacakKadi"] = kadi;
            return RedirectToAction("SifreYenile");
        }

        // 5. YENİ ŞİFRE BELİRLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult SifreYenile()
        {
            if (TempData["SifirlanacakKadi"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Kadi = TempData["SifirlanacakKadi"];
            TempData.Keep("SifirlanacakKadi"); // Veriyi bir sonraki istek için koru
            return View();
        }

        // 6. YENİ ŞİFREYİ KAYDET (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SifreYenile(string kadi, string yeniSifre, string sifreTekrar)
        {
            if (yeniSifre != sifreTekrar)
            {
                TempData["Hata"] = "Girdiğiniz şifreler uyuşmuyor! Lütfen tekrar deneyin.";
                ViewBag.Kadi = kadi;
                return View();
            }

            var yonetici = _context.Yoneticiler.FirstOrDefault(x => x.YoneticiKullaniciAdi == kadi);

            if (yonetici != null)
            {
                yonetici.YoneticiSifre = yeniSifre;
                _context.SaveChanges();

                TempData["Basarili"] = "Şifreniz başarıyla güncellendi. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            TempData["Hata"] = "Bir hata oluştu. Kullanıcı bulunamadı.";
            return View();
        }

        // 7. ÇIKIŞ YAP (LOGOUT)
        // [İster 25] : Güvenli çıkış ve oturum sonlandırma.
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}