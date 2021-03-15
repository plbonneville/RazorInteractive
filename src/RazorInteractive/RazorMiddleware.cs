using System;
using System.Linq;
using System.Text;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace RazorInteractive
{
    internal static class RazorMiddleware
    {
        private static readonly string[] DeaultImports = new[]
        {
            "System",
            "System.Text",
            "System.Collections",
            "System.Collections.Generic",
            "System.Linq",
            "System.Threading.Tasks"
        };

        public static void AddRazor(this Kernel kernel)
        {
            kernel.AddMiddleware(async (command, context, next) =>
            {
                if (command is SubmitCode sc
                    && context.Command is SubmitCode cell
                    && cell.Code.StartsWith("#!razor"))
                {
                    var model = context.HandlingKernel.CreateModelFromCurrentVariables();

                    var renderer = new RazorRenderer();

                    var code = DeaultImports
                        .Aggregate(new StringBuilder(),
                        (sb, @namespace) =>
                        {
                            sb.AppendLine($"@using {@namespace}");
                            return sb;
                        });

                    code.Append(sc.Code);

                    try
                    {
                        var result = await renderer.ParseAsync(code.ToString(), model);

                        context.Display(result, "text/html");
                    }
                    catch (Exception e)
                    {
                        context.DisplayStandardError(e.ToString());
                    }

                    return;
                }

                await next(command, context);
            });
        }
    }
}