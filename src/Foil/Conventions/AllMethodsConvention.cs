using System;
using System.Reflection;
using Foil.Interceptions;

namespace Foil.Conventions
{
    public class AllMethodsConvention : IMethodConvention
    {
        public bool ShouldIntercept(Type type, MethodInfo methodInfo)
        {
            return true;
        }
    }
}