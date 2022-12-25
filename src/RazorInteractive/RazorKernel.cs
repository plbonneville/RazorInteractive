using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace RazorInteractive;

/// <summary>
/// A <see cref="Kernel"/> that renders Razor markup.
/// </summary>
public class RazorKernel : Kernel, IKernelCommandHandler<SubmitCode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RazorKernel"/> class.
    /// </summary>
    /// <returns>A new <see cref="RazorKernel"/> instance.</returns>
    public RazorKernel() : base("razor")
    {
    }

    public Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {
        var markdown = new RazorMarkdown(command.Code);
        context.Display(markdown);
        return Task.CompletedTask;
    }
}