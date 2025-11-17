using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
            // Correct absolute file path
            var filePath = Path.Combine(_env.WebRootPath, "Static", "Icons", $"{Name}.svg");

            if (!File.Exists(filePath))
            {
                output.Content.SetContent($"<!-- Icon not found: {Name} -->");
                return;
            }

            string svg = File.ReadAllText(filePath);

            // Replace width/height dynamically
            svg = svg.Replace("width=\"25\"", $"width=\"{Size}\"")
                .Replace("height=\"25\"", $"height=\"{Size}\"");

            output.TagName = null;
            output.Content.SetHtmlContent(svg);
        }
    }
}