using System;
using Castle.DynamicProxy;

namespace Foil.Sample.Interceptors
{
    public class LogInterceptor : IInterceptor
    {
        public virtual void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Before {invocation.Method.Name}");
            
            invocation.Proceed();
            
            Console.WriteLine($"After {invocation.Method.Name}");
        }
    }
}