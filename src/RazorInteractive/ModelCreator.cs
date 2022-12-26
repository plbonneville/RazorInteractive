using System.Collections.Immutable;
using System.Dynamic;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.FSharp;

namespace RazorInteractive;

internal static class ModelCreator
{
    /// <summary>
    /// Create a model from the variables in the C# and F# kernels.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/>.</param>
    /// <returns>An <see cref="ExpandoObject"/> object that holds all available C# and F# variables.</returns>
    public static IDictionary<string, object> CreateModelFromCurrentVariables(this Kernel kernel)
    {
        var tempModel = kernel switch
        {
            CSharpKernel csharpKernel => csharpKernel.GetVariables(),
            FSharpKernel fsharpKernel => fsharpKernel.GetVariables(),
            CompositeKernel compositeKernel => compositeKernel.SelectMany(x => x.CreateModelFromCurrentVariables()),
            _ => ImmutableDictionary<string, object>.Empty
        };

        var model = new ExpandoObject() as IDictionary<string, object>;

        foreach (var item in tempModel)
        {
            model.TryAdd(item.Key, item.Value);
        }

        return model;
    }

    private static IDictionary<string, object> GetVariables(this CSharpKernel csharpKernel)
        => csharpKernel
            .ScriptState
            ?.Variables
            .Aggregate(new Dictionary<string, object>() as IDictionary<string, object>,
            (dictionary, variable) =>
            {
                dictionary.Add(variable.Name, variable.Value);
                return dictionary;
            }) ?? ImmutableDictionary<string, object>.Empty;

    private static IDictionary<string, object> GetVariables(this FSharpKernel fsharpKernel)
        => fsharpKernel
            .GetValues()
            .Aggregate(new Dictionary<string, object>() as IDictionary<string, object>,
            (dictionary, variable) =>
            {
                dictionary.Add(variable.Name, variable.Value);
                return dictionary;
            });
}
