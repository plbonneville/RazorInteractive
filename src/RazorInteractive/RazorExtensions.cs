using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;

namespace RazorInteractive;

public static class RazorExtensions
{
    private static readonly string[] DefaultImports = new[]
    {
        "System",
        "System.Text",
        "System.Collections",
        "System.Collections.Generic",
        "System.Linq",
        "System.Threading.Tasks"
    };

    public static T UseRazor<T>(this T kernel) where T : Kernel
    {
        Formatter.Register<RazorMarkdown>((markdown, writer) =>
        {
            IHtmlContent html = new HtmlString("");

            var model = kernel.CreateModelFromCurrentVariables();

            Task.Run(async () => html = await GenerateHtmlAsync(markdown.Value, model))
            .Wait();

            html.WriteTo(writer, HtmlEncoder.Default);

        }, HtmlFormatter.MimeType);

        return kernel;
    }

    private static async Task<IHtmlContent> GenerateHtmlAsync(string submittedMarkdown, IDictionary<string, object> model)
    {
        var renderer = new RazorRenderer();

        var code = DefaultImports
            .Aggregate(new StringBuilder(),
            (sb, @namespace) => sb.AppendLine($"@using {@namespace}"));

        code.Append(submittedMarkdown);

        return await renderer.ParseAsync(code.ToString(), model);
    }
}
