using Castle.DynamicProxy;

namespace Foil.Interceptions
{
    public interface IMethodSelectionConvenstion
    {
        bool HasSupport(IInvocation invocation);
    }
}