using System;
using System.Linq;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Foil.Conventions;
using Foil.Interceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Foil
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientWithInterception<T, TImplementation>(
            this IServiceCollection services, Action<IInterceptBy> action)
            where T : class
            where TImplementation : class, T
        {
            return AddWithInterception<T, TImplementation>(services, action, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScopedWithInterception<T, TImplementation>(
            this IServiceCollection services, Action<IInterceptBy> action)
            where T : class
            where TImplementation : class, T
        {
            return AddWithInterception<T, TImplementation>(services, action, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddSingletonWithInterception<T, TImplementation>(
            this IServiceCollection services, Action<IInterceptBy> action)
            where T : class
            where TImplementation : class, T
        {
            return AddWithInterception<T, TImplementation>(services, action, ServiceLifetime.Singleton);
        }

        private static IServiceCollection AddWithInterception<T, TImplementation>(this IServiceCollection services,
            Action<IInterceptBy> action, ServiceLifetime lifetime)
            where T : class
            where TImplementation : class, T
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (!Enum.IsDefined(typeof(ServiceLifetime), lifetime))
                throw new ArgumentOutOfRangeException(nameof(lifetime),
                    "Value should be defined in the ServiceLifetime enum.");

            var interceptionOptions = new InterceptionOptions();
            action?.Invoke(interceptionOptions);

            interceptionOptions.Interceptors.ForEach(services.TryAddTransient);

            services.TryAdd(ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime));

            services.AddTransient(sp =>
            {
                var interceptorInstances = interceptionOptions.Interceptors
                    .Select(sp.GetRequiredService)
                    .Cast<IInterceptor>()
                    .ToArray();
                
                var implementation = sp.GetRequiredService<TImplementation>();

                var proxyFactory = new ProxyGenerator();
                var proxyGenerationHook = new ConvensionBasedProxyGenerationHook(interceptionOptions.Convention);
                var proxyGenerationOptions = new ProxyGenerationOptions(proxyGenerationHook);
                
                return proxyFactory.CreateInterfaceProxyWithTarget<T>(implementation, proxyGenerationOptions, interceptorInstances);
            });

            return services;
        }
    }
}