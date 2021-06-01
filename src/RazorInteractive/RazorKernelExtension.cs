using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;

namespace RazorInteractive
{
    public class RazorKernelExtension : IKernelExtension, IStaticContentSource
    {
        public string Name => "Razor";

        public async Task OnLoadAsync(Kernel kernel)
        {
            if (kernel is CompositeKernel compositeKernel)
            {
                compositeKernel.Add(new RazorKernel());
            }

            kernel.UseRazor();

            var message = new HtmlString(@"
<details>
    <summary>Renders the code block as Razor markup in dotnet-interactive notebooks.</summary>
    <p>This extension adds a new kernel that can render Razor markdown.</p>
</details>");

            var formattedValue = new FormattedValue(
                HtmlFormatter.MimeType,
                message.ToDisplayString(HtmlFormatter.MimeType));

            await kernel.SendAsync(new DisplayValue(formattedValue, Guid.NewGuid().ToString()));
        }
    }
}
