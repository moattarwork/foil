using System;
using Foil.Conventions;
using Foil.UnitTests.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Foil.UnitTests
{
    public class ServiceCollectionFixture
    {
        public IServiceProvider ProviderWithTransientOf<TContract, T>()
            where TContract : class where T : class, TContract
        {
            var logger = Substitute.For<ISampleLogger>();
            var services = new ServiceCollection();

            services.AddSingleton(logger);
            services.AddTransientWithInterception<TContract, T>(m => m.InterceptBy<LogInterceptor>());
            return services.BuildServiceProvider();
        }

        public IServiceProvider ProviderWithConventionBasedTransientOf<TContract, T>()
            where TContract : class where T : class, TContract
        {
            var logger = Substitute.For<ISampleLogger>();
            var services = new ServiceCollection();

            services.AddSingleton(logger);
            services.AddTransientWithInterception<TContract, T>(m =>
                m.InterceptBy<LogInterceptor>().UseMethodConvention<NonQueryMethodsConvention>());
            return services.BuildServiceProvider();
        }

        public IServiceProvider ProviderWithSingletonOf<TContract, T>()
            where TContract : class where T : class, TContract
        {
            var logger = Substitute.For<ISampleLogger>();
            var services = new ServiceCollection();

            services.AddSingleton(logger);
            services.AddSingletonWithInterception<TContract, T>(m => m.InterceptBy<LogInterceptor>());
            return services.BuildServiceProvider();
        }
    }
}