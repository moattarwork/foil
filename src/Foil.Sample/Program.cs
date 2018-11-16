using Foil.Logging;
using Foil.Sample.Interceptors;
using Foil.Sample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Foil.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddTransientWithInterception<ISampleService, SampleService>(m => m.InterceptBy<LoggingInterceptor>().ThenBy<LogInterceptor>());

            var provider = services.BuildServiceProvider();
            var loggingFactory = provider.GetRequiredService<ILoggerFactory>();
            loggingFactory.AddConsole(LogLevel.Trace);

            var service = provider.GetRequiredService<ISampleService>();
            
            service.Call("Dear User");
        }
    }
}