# foil

[![Build status](https://ci.appveyor.com/api/projects/status/x97rqf3f82647e1j?svg=true)](https://ci.appveyor.com/project/moattarwork/foil-nha98)
[![NuGet Status](https://img.shields.io/nuget/v/Foil.svg)](https://www.nuget.org/packages/Foil/)

foil is a set of extensions which enable interception support for .Net Core dependency injection framework. It uses Castle Core framework to enable on the fly proxy creation of container elements.

The package can be downloaded from NuGet using

```
install-package Foil
install-package Foil.Logging
```

or 

```
dotnet add package Foil
dotnet add package Foil.Logging
```


## Usage
The package consists of extensions to register services as Transient, Scoped or Singleton with the interceptors.

```c#
services.AddTransientWithInterception<ISampleService, SampleService>(m => m.InterceptBy<LogInterceptor>()
    .UseMethodConvention<NonQueryMethodsConvention>());
```  
or
```c#
services.AddSingletonWithInterception<ISampleService, SampleService>(m => m.InterceptBy<LogInterceptor>()
    .UseMethodConvention<NonQueryMethodsConvention>());

```
Convention give this option to specify which methods need to be selected for interception. The are couple of predefined convention which can be used:
- AllMethodsConvention (Default)
- NonQueryMethodsConvention

Custom conventions can be provided by implementing IMethodConvention.

## Code sample

```c#
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

public interface ISampleService
{
    void Call();
    string State { get; }
}

public class SampleService : ISampleService
{
    public string State { get; private set; } = string.Empty;
    
    public virtual void Call()
    {
        State = "Changed";
        Console.WriteLine("Hello Sample");
    }
}

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
```
    
