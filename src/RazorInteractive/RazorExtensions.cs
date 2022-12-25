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

    /// <summary>
    /// Registers the type <see cref="RazorMarkdown"/> as a formatter.
    /// </summary>
    /// <param name="kernel">A <see cref="Kernel"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static T UseRazor<T>(this T kernel) where T : Kernel
    {
        Formatter.Register<RazorMarkdown>((markdown, writer) =>
        {
            IHtmlContent html = new HtmlString(string.Empty);

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
