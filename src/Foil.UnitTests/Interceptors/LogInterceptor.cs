using Castle.DynamicProxy;

namespace Foil.UnitTests.Interceptors
{
    public class LogInterceptor : IInterceptor
    {
        private readonly ISampleLogger _logger;

        public LogInterceptor(ISampleLogger logger)
        {
            _logger = logger;
        }

        public virtual void Intercept(IInvocation invocation)
        {
            _logger.Log("Before invocation");
            
            invocation.Proceed();
            
            _logger.Log("After invocation");
        }
    }
}