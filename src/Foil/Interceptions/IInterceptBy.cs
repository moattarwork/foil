using Castle.DynamicProxy;

namespace Foil.Interceptions
{
    public interface IInterceptBy
    {
        IThenInterceptBy InterceptBy<TInterceptor>() where TInterceptor : IInterceptor;
    }
}