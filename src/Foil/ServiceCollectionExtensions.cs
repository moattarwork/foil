using System;
using System.Linq;
using Castle.DynamicProxy;
using Foil.Conventions;
using Foil.Interceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Foil
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientWithInterception<T, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TImplementation> serviceFactory,
            Action<IInterceptBy> configurator)
            where T : class where TImplementation : class, T
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            return services.Add<T, TImplementation>(
                lifetime => new ServiceDescriptor(typeof(TImplementation), serviceFactory, lifetime),
                configurator, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScopedWithInterception<T, TImplementation>(this IServiceCollection services,
            Func<IServiceProvider, TImplementation> serviceFactory,
            Action<IInterceptBy> configurator)
            where T : class where TImplementation : class, T
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            return services.Add<T, TImplementation>(
                lifetime => new ServiceDescriptor(typeof(TImplementation), serviceFactory, lifetime),
                configurator, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddSingletonWithInterception<T, TImplementation>(
            this IServiceCollection services,
            Func<IServiceProvider, TImplementation> serviceFactory,
            Action<IInterceptBy> configurator)
            where T : class where TImplementation : class, T
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            return services.Add<T, TImplementation>(
                lifetime => new ServiceDescriptor(typeof(TImplementation), serviceFactory, lifetime),
                configurator, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddTransientWithInterception<T, TImplementation>(
            this IServiceCollection services, Action<IInterceptBy> configurator)
            where T : class
            where TImplementation : class, T
        {
            return Add<T, TImplementation>(services,
                lifetime => ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime),
                configurator, ServiceLifetime.Transient);
        }

        public static IServiceCollection AddScopedWithInterception<T, TImplementation>(
            this IServiceCollection services, Action<IInterceptBy> configurator)
            where T : class
            where TImplementation : class, T
        {
            return Add<T, TImplementation>(services,
                lifetime => ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime),
                configurator, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddSingletonWithInterception<T, TImplementation>(
            this IServiceCollection services, Action<IInterceptBy> configurator)
            where T : class
            where TImplementation : class, T
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            return Add<T, TImplementation>(services,
                lifetime => ServiceDescriptor.Describe(typeof(TImplementation), typeof(TImplementation), lifetime),
                configurator, ServiceLifetime.Singleton);
        }

        private static IServiceCollection Add<TService, TImplementation>(this IServiceCollection services,
            Func<ServiceLifetime, ServiceDescriptor> descriptorFactory, Action<IInterceptBy> configurator,
            ServiceLifetime lifetime) where TService : class where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configurator == null) throw new ArgumentNullException(nameof(configurator));


            var interceptionOptions = new InterceptionOptions();
            configurator.Invoke(interceptionOptions);

            interceptionOptions.Interceptors.ForEach(services.TryAddTransient);
            services.TryAdd(descriptorFactory(lifetime));

            services.Add(ServiceDescriptor.Describe(typeof(TService), sp =>
            {
                var interceptorInstances = interceptionOptions.Interceptors
                    .Select(sp.GetRequiredService)
                    .Cast<IInterceptor>()
                    .ToArray();

                var implementation = sp.GetRequiredService<TImplementation>();

                var proxyFactory = new ProxyGenerator();
                var proxyGenerationHook = new ConventionBasedProxyGenerationHook(interceptionOptions.Convention);
                var proxyGenerationOptions = new ProxyGenerationOptions(proxyGenerationHook);

                return proxyFactory.CreateInterfaceProxyWithTarget<TService>(implementation, proxyGenerationOptions,
                    interceptorInstances);
            }, lifetime));

            return services;
        }
    }
}