using Foil.Interceptions;

namespace Foil.Conventions
{
    public class MethodConvenstions
    {
        public static IMethodConvention Default => new AllMethodsConvention();

        public static IMethodConvention NonQueryMethods => new NonQueryMethodsConvention();
    }
}