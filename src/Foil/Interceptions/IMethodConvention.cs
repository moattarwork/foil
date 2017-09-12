using System;
using System.Reflection;

namespace Foil.Interceptions
{
    public interface IMethodConvention
    {
        bool ShouldIntercept(Type type, MethodInfo methodInfo);
    }
}