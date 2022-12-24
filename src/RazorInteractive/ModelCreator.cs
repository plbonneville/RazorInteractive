using System.Dynamic;

using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.FSharp;

namespace RazorInteractive;

internal static class ModelCreator
{
    public static IDictionary<string, object> CreateModelFromCurrentVariables(this Kernel kernel)
    {
        var model = new ExpandoObject() as IDictionary<string, object>;

        if (kernel is CompositeKernel compositeKernel)
        {
            foreach (var childKernel in compositeKernel.ChildKernels)
            {
                var tempModel = childKernel.CreateModelFromCurrentVariables();

                foreach (var item in tempModel)
                {
                    model.TryAdd(item.Key, item.Value);
                }
            }
        }
        else if (kernel is CSharpKernel csharpKernel)
        {
            var tempModel = csharpKernel.CreateModelFromCurrentVariables();

            foreach (var item in tempModel)
            {
                model.TryAdd(item.Key, item.Value);
            }
        }
        else if (kernel is FSharpKernel fsharpKernel)
        {
            var tempModel = fsharpKernel.CreateModelFromCurrentVariables();

            foreach (var item in tempModel)
            {
                model.TryAdd(item.Key, item.Value);
            }
        }

        return model;
    }

    private static IDictionary<string, object> CreateModelFromCurrentVariables(this CSharpKernel csharpKernel)
        => csharpKernel
            .ScriptState
            ?.Variables
            .Aggregate(new ExpandoObject() as IDictionary<string, object>,
            (dictionary, variable) =>
            {
                dictionary.Add(variable.Name, variable.Value);
                return dictionary;
            }) ?? new Dictionary<string, object>();

    private static IDictionary<string, object> CreateModelFromCurrentVariables(this FSharpKernel fsharpKernel)
        => fsharpKernel
            .GetValues()
            ?.Aggregate(new ExpandoObject() as IDictionary<string, object>,
            (dictionary, variable) =>
            {
                dictionary.Add(variable.Name, variable.Value);
                return dictionary;
            }) ?? new Dictionary<string, object>();
}
