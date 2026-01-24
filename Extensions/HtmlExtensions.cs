using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestoranProje1.Extensions
{
    public static class HtmlExtensions
    {
        // [İster 10] : Custom Html Helper Kanıtı
        public static IHtmlContent MetinKisalt(this IHtmlHelper htmlHelper, string metin, int uzunluk = 50)
        {
            // Metin boşsa veya zaten kısaysa olduğu gibi döndür
            if (string.IsNullOrEmpty(metin) || metin.Length <= uzunluk)
            {
                return new HtmlString(metin);
            }

            // Metni kes ve sonuna üç nokta ekle
            string kisaMetin = metin.Substring(0, uzunluk) + "...";

            // Mouse ile üzerine gelince tamamı görünsün 
            return new HtmlString($"<span title='{metin}'>{kisaMetin}</span>");
        }
    }
}