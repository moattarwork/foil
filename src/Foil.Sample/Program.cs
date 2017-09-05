using Foil.Sample.Interceptors;
using Foil.Sample.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Foil.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddTransientWithInterception<ISampleService, SampleService>(m => m.InterceptBy<LogInterceptor>());

            var provider = services.BuildServiceProvider();

            var service = provider.GetRequiredService<ISampleService>();
            service.Call();
        }
    }
}