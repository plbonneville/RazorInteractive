using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Formatting;

namespace RazorInteractive;

public class RazorKernelExtension : IKernelExtension, IStaticContentSource
{
    public string Name => "Razor";

    /// <summary>
    /// When the composite kernel is loaded, add a Razor kernel and register the Razor formatter.
    /// </summary>
    /// <param name="kernel">Must be of type <see cref="CompositeKernel"/>.</param>
    public async Task OnLoadAsync(Kernel kernel)
    {
        if (kernel is not CompositeKernel compositeKernel)
        {
            throw new InvalidOperationException("The Razor kernel can only be added into a CompositeKernel.");
        }

        // Add a RazorKernel as a child kernel to the CompositeKernel
        compositeKernel.Add(new RazorKernel());

        compositeKernel.UseRazor();

        var message = new HtmlString(
            """
			<details>
				<summary>Renders the code block as Razor markup in dotnet-interactive notebooks.</summary>
				<p>This extension adds a new kernel that can render Razor markdown.</p>
			</details>
			""");

        var formattedValue = new FormattedValue(
            HtmlFormatter.MimeType,
            message.ToDisplayString(HtmlFormatter.MimeType));

        await compositeKernel.SendAsync(new DisplayValue(formattedValue, Guid.NewGuid().ToString()));
    }
}
