using Castle.DynamicProxy;

namespace Foil.Interceptions
{
    public interface IThenInterceptBy : IUseMethodConvention
    {
        IThenInterceptBy ThenBy<TInterceptor>() where TInterceptor : IInterceptor;
    }
}