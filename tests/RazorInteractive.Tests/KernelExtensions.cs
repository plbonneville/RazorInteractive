using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.FSharp;
using Microsoft.DotNet.Interactive.PackageManagement;

namespace RazorInteractive.Tests;

public static class KernelExtensions
{
    public static CSharpKernel UseNugetDirective(this CSharpKernel kernel, bool forceRestore = false)
    {
        kernel.UseNugetDirective((k, resolvedPackageReference) =>
        {

            k.AddAssemblyReferences(resolvedPackageReference
                .SelectMany(r => r.AssemblyPaths));
            return Task.CompletedTask;
        }, forceRestore);

        return kernel;
    }

    public static FSharpKernel UseNugetDirective(this FSharpKernel kernel, bool forceRestore = false)
    {
        kernel.UseNugetDirective((k, resolvedPackageReference) =>
        {
            var resolvedAssemblies = resolvedPackageReference
                .SelectMany(r => r.AssemblyPaths);

            var packageRoots = resolvedPackageReference
                .Select(r => r.PackageRoot);


            k.AddAssemblyReferencesAndPackageRoots(resolvedAssemblies, packageRoots);
            return Task.CompletedTask;
        }, forceRestore);

        return kernel;
    }
}
