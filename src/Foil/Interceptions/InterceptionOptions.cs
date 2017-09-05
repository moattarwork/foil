using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Foil.Conventions;

namespace Foil.Interceptions
{
    public class InterceptionOptions : IInterceptBy, IThenInterceptBy
    {
        private readonly IDictionary<Type, Type> _interceptors = new Dictionary<Type, Type>();

        public void UseMethodSelectionConvention<TConvention>() where TConvention : IMethodSelectionConvenstion, new()
        {
            Convention = new TConvention();
        }

        public IThenInterceptBy ThenBy<TInterceptor>() where TInterceptor : IInterceptor
        {
            if (_interceptors.ContainsKey(typeof(TInterceptor)))
                _interceptors.Add(typeof(TInterceptor), typeof(TInterceptor));

            return this;
        }

        public IThenInterceptBy InterceptBy<TInterceptor>() where TInterceptor : IInterceptor
        {
            _interceptors.Clear();
            
            _interceptors.Add(typeof(TInterceptor), typeof(TInterceptor));

            return this;
        }

        public List<Type> Interceptors => _interceptors.Values.ToList();

        public IMethodSelectionConvenstion Convention { get; private set; } = new DefaultMethodSelectionConvenstion();
    }
}