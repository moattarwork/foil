using System;
using System.Linq;
using System.Reflection;
using Foil.Interceptions;

namespace Foil.Conventions
{
    public class NonQueryMethodsConvention : IMethodConvention
    {
        private readonly string[] _prefixes = {"Get", "Load"};
        
        public bool ShouldIntercept(Type type, MethodInfo methodInfo)
        {
            return !_prefixes.Any(p => methodInfo.Name.StartsWith(p, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}