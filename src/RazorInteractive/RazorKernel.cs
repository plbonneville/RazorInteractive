using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace RazorInteractive;

public class RazorKernel : Kernel, IKernelCommandHandler<SubmitCode>
{
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