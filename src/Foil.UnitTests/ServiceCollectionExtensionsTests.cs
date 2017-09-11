using FluentAssertions;
using Foil.UnitTests.Interceptors;
using Foil.UnitTests.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Foil.UnitTests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void Should_AddTransientWithInterception_CreateTheTransientInstanceWithInterception()
        {
            // Given
            var logger = Substitute.For<ISampleLogger>();
            var services = new ServiceCollection();
            
            services.AddSingleton(logger);
            services.AddTransientWithInterception<ISampleService, SampleService>(m => m.InterceptBy<LogInterceptor>());
            var provider = services.BuildServiceProvider();
            var sut = provider.GetRequiredService<ISampleService>();
            var secondSut = provider.GetRequiredService<ISampleService>();

            // When            
            sut.Call();
            
            // Then
            logger.Received(1).Log("Before invocation");
            logger.Received(1).Log("After invocation");

            secondSut.Should().NotBeSameAs(sut);
            sut.State.Should().Be("Changed");
            secondSut.State.Should().Be(string.Empty);
        }        
        
        [Fact]
        public void Should_AddSingletonWithInterception_CreateTheSingletonInstanceWithInterception()
        {
            // Given
            var logger = Substitute.For<ISampleLogger>();
            var services = new ServiceCollection();
            
            services.AddSingleton(logger);
            services.AddSingletonWithInterception<ISampleService, SampleService>(m => m.InterceptBy<LogInterceptor>());
            var provider = services.BuildServiceProvider();
            var sut = provider.GetRequiredService<ISampleService>();
            var secondSut = provider.GetRequiredService<ISampleService>();

            // When            
            sut.Call();
            
            // Then
            logger.Received(1).Log("Before invocation");
            logger.Received(1).Log("After invocation");

            secondSut.Should().NotBeSameAs(sut);
            sut.State.Should().Be("Changed");
            secondSut.State.Should().Be("Changed");
        }
    }
}