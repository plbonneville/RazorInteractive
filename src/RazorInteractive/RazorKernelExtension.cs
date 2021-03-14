using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

using Microsoft.DotNet.Interactive;

namespace RazorInteractive
{
    public class RazorKernelExtension : IKernelExtension
    {
        public Task OnLoadAsync(Kernel kernel)
        {
            var razorCommand = new Command("#!razor", "Renders the code block as Razor markup.")
            {
                Handler = CommandHandler.Create<KernelInvocationContext>(context =>
                {
                    context.HandlingKernel.AddRazor();
                })
            };

            kernel.AddDirective(razorCommand);

            return Task.CompletedTask;
        }
    }
}
