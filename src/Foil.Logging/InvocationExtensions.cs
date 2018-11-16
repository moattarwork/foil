using System;
using System.Linq;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace Foil.Logging
{
    public static class InvocationExtensions
    {
        public static string FormatArguments(this IInvocation invocation)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));

            var arguments = invocation.Arguments;
            if (!arguments.Any())
                return string.Empty;

            
            var serializedArguments = arguments
                .Select((a, index) => $"Arg[{index}]: {a.ToJsonOrDefault()}")
                .ToList();
            return string.Join(Environment.NewLine, serializedArguments);

        }

        private static string ToJsonOrDefault(this object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return "Error in serializing argument to json";
            }
        }
    }
}