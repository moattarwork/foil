using System;
using System.Reflection;
using Castle.DynamicProxy;
using Foil.Interceptions;
using Microsoft.Extensions.Logging;

namespace Foil.Conventions
{
    public class ConvensionBasedProxyGenerationHook : IProxyGenerationHook
    {
        private readonly IMethodConvention _convention;

        public ConvensionBasedProxyGenerationHook(IMethodConvention convention)
        {
            _convention = convention ?? throw new ArgumentNullException(nameof(convention));
        }
        
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return _convention.ShouldIntercept(type, methodInfo);
        }
    }
}