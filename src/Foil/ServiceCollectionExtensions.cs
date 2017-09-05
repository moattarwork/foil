using System;
using System.Linq;
using Castle.DynamicProxy;
using Foil.Interceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Foil
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientWithInterception<T, TImplementation>(this IServiceCollection services, Action<IInterceptBy> action) 
            where T : class
            where TImplementation: class, T
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var interceptionOptions = new InterceptionOptions();
            action?.Invoke(interceptionOptions);

            interceptionOptions.Interceptors.ForEach(services.TryAddTransient);
            services.TryAddTransient<TImplementation>();

            services.AddTransient(sp =>
            {
                var interceptorInstances = interceptionOptions.Interceptors.Select(sp.GetRequiredService).Cast<IInterceptor>().ToArray();
                var implementation = sp.GetRequiredService<TImplementation>();
                
                var proxyFactory = new ProxyGenerator();
                return proxyFactory.CreateInterfaceProxyWithTarget<T>(implementation, interceptorInstances);
            });

            return services;
        }
        
        
    }
}