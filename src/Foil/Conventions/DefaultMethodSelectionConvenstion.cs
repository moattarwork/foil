using Castle.DynamicProxy;
using Foil.Interceptions;

namespace Foil.Conventions
{
    public class DefaultMethodSelectionConvenstion : IMethodSelectionConvenstion
    {
        public bool HasSupport(IInvocation invocation)
        {
            return true;
        }
    }
}