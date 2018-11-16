using System;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace Foil.Logging
{
    public sealed class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void Intercept(IInvocation invocation)
        {
            _logger.LogInformation($"Executing Method: {invocation.Method.Name}");

            LogInvocationInfo(invocation);
            
            invocation.Proceed();
            
            _logger.LogInformation($"Executed Method: {invocation.Method.Name}");
        }

        private void LogInvocationInfo(IInvocation invocation)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace(invocation.FormatArguments());
        }
    }
}