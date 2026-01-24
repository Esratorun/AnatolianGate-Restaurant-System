using Microsoft.AspNetCore.Mvc;
using RestoranProje1.Models;

namespace RestoranProje1.ViewComponents
{
    // [İster 24] : ViewComponent Kullanımı
    public class AscilarViewComponent : ViewComponent
    {
        private readonly RestoranContext _context;

        // [İster 17] : Dependency Injection
        public AscilarViewComponent(RestoranContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var ascilar = _context.Ascilar.ToList();
            return View(ascilar);
        }
    }
}