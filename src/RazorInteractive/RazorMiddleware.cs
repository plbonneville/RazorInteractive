using System;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;

namespace RazorInteractive
{
    internal static class RazorMiddleware
    {
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

                    try
                    {
                        var result = await renderer.ParseAsync(sc.Code, model);

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