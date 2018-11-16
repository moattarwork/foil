using System;
using System.Reflection;
using Castle.DynamicProxy;
using Foil.Interceptions;

namespace Foil.Conventions
{
    public class ConventionBasedProxyGenerationHook : IProxyGenerationHook
    {
        private readonly IMethodConvention _convention;

        public ConventionBasedProxyGenerationHook(IMethodConvention convention)
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