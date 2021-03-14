using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using Microsoft.DotNet.Interactive;

namespace RazorInteractive
{
    internal static class ModelCreator
    {
        public static IDictionary<string, object> CreateModelFromCurrentVariables(this Kernel kernel)
            => CreateModelFromCurrentVariables(kernel as DotNetKernel);

        private static IDictionary<string, object> CreateModelFromCurrentVariables(this DotNetKernel dotNetKernel)
            => dotNetKernel
                ?.GetVariableNames()
                .Select(name =>
                {
                    dotNetKernel.TryGetVariable(name, out object value);
                    return new CurrentVariable(name, value.GetType(), value);
                })
                .Aggregate(new ExpandoObject() as IDictionary<string, object>,
                (dictionary, variable) =>
                {
                    dictionary.Add(variable.Name, variable.Value);
                    return dictionary;
                }) ?? new Dictionary<string, object>();
    }
}
