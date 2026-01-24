using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RestoranProje1.TagHelpers
{
    // [İster 13] : Custom Tag Helper (Özel Etiket Yardımcısı)
    // Kullanımı: <fiyat deger="150.50"></fiyat>
    [HtmlTargetElement("fiyat")]
    public class FiyatTagHelper : TagHelper
    {
        public decimal Deger { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            // Tasarım ayarları (Tailwind CSS)
            output.Attributes.SetAttribute("class", "text-yellow-600 font-bold text-lg border border-yellow-200 px-2 py-1 rounded bg-yellow-50");

            // Para birimi formatı (₺)
            output.Content.SetContent(Deger.ToString("C2"));
        }
    }
}