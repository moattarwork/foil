using FluentAssertions;
using Foil.UnitTests.Interceptors;
using Foil.UnitTests.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Foil.UnitTests
{
    public class ServiceCollectionExtensionsTests : IClassFixture<ServiceCollectionFixture>
    {
        private readonly ServiceCollectionFixture _fixture;

        public ServiceCollectionExtensionsTests(ServiceCollectionFixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void Should_AddSingletonWithInterception_CreateTheSingletonInstanceWithInterception()
        {
            // Given
            var provider = _fixture.ProviderWithSingletonOf<ISampleService, SampleService>();
            var logger = provider.GetRequiredService<ISampleLogger>();
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

        [Fact]
        public void Should_AddTransientWithInterception_CreateTheTransientInstanceWithInterception()
        {
            // Given
            var provider = _fixture.ProviderWithTransientOf<ISampleService, SampleService>();
            var logger = provider.GetRequiredService<ISampleLogger>();
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
        public void Should_AddTransientWithInterception_WhenSpecifyNonQueryConvention_InterceptionIsAvailableOnNonQueryMethods()
        {
            // Given
            var provider = _fixture.ProviderWithConventionBasedTransientOf<ISampleService, SampleService>();
            var logger = provider.GetRequiredService<ISampleLogger>();
            var sut = provider.GetRequiredService<ISampleService>();

            // When            
            sut.Call();

            // Then
            logger.Received(1).Log("Before invocation");
            logger.Received(1).Log("After invocation");
        }        
        
        [Fact]
        public void Should_AddTransientWithInterception_WhenSpecifyNonQueryConvention_NoInterceptionOnQueryMethods()
        {
            // Given
            var provider = _fixture.ProviderWithConventionBasedTransientOf<ISampleService, SampleService>();
            var logger = provider.GetRequiredService<ISampleLogger>();
            var sut = provider.GetRequiredService<ISampleService>();

            // When            
            sut.GetName();

            // Then
            logger.DidNotReceive().Log(Arg.Any<string>());
            logger.DidNotReceive().Log(Arg.Any<string>());
        }
    }
}