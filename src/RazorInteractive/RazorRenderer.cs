using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive.Formatting;
using RazorLight;

namespace RazorInteractive;

internal sealed class RazorRenderer
{
    private readonly RazorLightEngine _engine;

    public RazorRenderer()
    {
        _engine = new RazorLightEngineBuilder()
            // required to have a default RazorLightProject type,
            // but not required to create a template from string.
            .UseEmbeddedResourcesProject(typeof(RazorRenderer))
            .SetOperatingAssembly(typeof(RazorRenderer).Assembly)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<IHtmlContent> ParseAsync(string template, IDictionary<string, object> model)
    {
        var result = await _engine.CompileRenderStringAsync(GetHashString(template), template, model);

        var id = "razorExtension" + Guid.NewGuid().ToString("N");

        return Html.ToHtmlContent($"""<div id="{id}">{result}</div>""");
    }

    private static string GetHashString(string inputString)
        => SHA256.HashData(Encoding.UTF8.GetBytes(inputString))
        .Aggregate(new StringBuilder(),
            (sb, b) => sb.Append(b.ToString("X2")))
        .ToString();
}