using System.Linq;
using System.Reflection;
using FluentAssertions;
using Foil.Conventions;
using Xunit;

namespace Foil.UnitTests.Conventions
{
    public class NonQueryMethodsConventionTests
    {
        [Fact]
        public void Should_ShouldIntercept_ReturnTrueForAllMethods()
        {
            // Given
            var type = typeof(SampleInterceptee);
            var methodInfos = type.GetMethods();
            var sut = new NonQueryMethodsConvention();

            // When
            var actuals = methodInfos.Select(m => new { Actual = sut.ShouldIntercept(type, m), Name = m.Name});

            // Then
            actuals.Should().HaveCount(methodInfos.Length)
                .And.Contain(m => !m.Actual && m.Name == "LoadList")
                .And.Contain(m => !m.Actual && m.Name == "GetList")
                .And.Contain(m => m.Actual && m.Name == "Insert");
        }
    }
}