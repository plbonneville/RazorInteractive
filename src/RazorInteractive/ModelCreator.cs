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
            else if (kernel is DotNetKernel dotnetKernel)
            {
                var tempModel = dotnetKernel.CreateModelFromCurrentVariables();

                foreach (var item in tempModel)
                {
                    model.TryAdd(item.Key, item.Value);
                }
            }

            return model;
        }

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
