using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace FeedHiveAuth.CustomTagHelpers
{
    [HtmlTargetElement("icon")]
    public class IconTagHelper : TagHelper
    {
        private readonly IWebHostEnvironment _env;

        public IconTagHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string Name { get; set; }
        public int Size { get; set; } = 25;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var filePath = Path.Combine(_env.WebRootPath, "Static", "Icons", $"{Name}.svg");

            if (!File.Exists(filePath))
            {
                output.TagName = null;
                output.Content.SetHtmlContent($"<!-- Icon not found: {Name} -->");
                return;
            }

            string svg = File.ReadAllText(filePath);

            if (svg.Contains("<image") || svg.Contains("<pattern") || svg.Contains("xlink:href"))
            {
                output.TagName = null;
                output.Content.SetHtmlContent(svg);   // FIXED
                return;
            }

            // Remove any width/height attributes
            svg = Regex.Replace(svg, "width=\"[^\"]*\"", "");
            svg = Regex.Replace(svg, "height=\"[^\"]*\"", "");

            // Insert your size
            svg = Regex.Replace(svg, "<svg", $"<svg width=\"{Size}\" height=\"{Size}\"");

            // Normalize stroke and fill
            svg = Regex.Replace(svg, "stroke=\"[^\"]*\"", "stroke=\"currentColor\"", RegexOptions.IgnoreCase);

            svg = Regex.Replace(svg, "fill=\"(?!(none|transparent))[^\"]*\"", "fill=\"currentColor\"", RegexOptions.IgnoreCase);

            output.TagName = null;  // remove <icon> wrapper
            output.Content.SetHtmlContent(svg);  // FIXED — no HtmlString wrapper
        }
    }
}