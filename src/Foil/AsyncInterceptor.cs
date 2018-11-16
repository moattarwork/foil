using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Foil
{
    public abstract class AsyncInterceptor : IInterceptor
    {
        private static readonly MethodInfo InterceptSynchronousMethodInfo =
            typeof(AsyncInterceptor)
                .GetMethod(nameof(InterceptSynchronousResult), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly ConcurrentDictionary<Type, GenericSynchronousHandler> GenericSynchronousHandlers =
            new ConcurrentDictionary<Type, GenericSynchronousHandler>
            {
                [typeof(void)] = InterceptSynchronousVoid
            };

        private static readonly MethodInfo HandleAsyncMethodInfo =
            typeof(AsyncInterceptor)
                .GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly ConcurrentDictionary<Type, GenericAsyncHandler> GenericAsyncHandlers =
            new ConcurrentDictionary<Type, GenericAsyncHandler>();

        public void Intercept(IInvocation invocation)
        {
            var methodType = GetMethodType(invocation.Method.ReturnType);

            switch (methodType)
            {
                case MethodType.AsyncAction:
                    InterceptAsynchronous(invocation);
                    return;
                case MethodType.AsyncFunction:
                    GetHandler(invocation.Method.ReturnType).Invoke(invocation, this);
                    return;
                default:
                    InterceptSynchronous(invocation);
                    return;
            }
        }

        private static MethodType GetMethodType(Type returnType)
        {
            if (returnType == typeof(void) || !typeof(Task).IsAssignableFrom(returnType))
                return MethodType.Synchronous;

            return returnType.GetTypeInfo().IsGenericType ? MethodType.AsyncFunction : MethodType.AsyncAction;
        }

        private static GenericAsyncHandler GetHandler(Type returnType)
        {
            var handler = GenericAsyncHandlers.GetOrAdd(returnType, CreateHandlerAsync);
            return handler;
        }

        private static void HandleAsyncWithResult<TResult>(IInvocation invocation,
            AsyncInterceptor asyncInterceptor)
        {
            asyncInterceptor.InterceptAsynchronous<TResult>(invocation);
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            var handler = GenericSynchronousHandlers.GetOrAdd(returnType, CreateHandlerForSync);
            handler(this, invocation);
        }

        private static GenericSynchronousHandler CreateHandlerForSync(Type returnType)
        {
            var method = InterceptSynchronousMethodInfo.MakeGenericMethod(returnType);
            return (GenericSynchronousHandler) method.CreateDelegate(typeof(GenericSynchronousHandler));
        }

        private static GenericAsyncHandler CreateHandlerAsync(Type returnType)
        {
            var taskReturnType = returnType.GetGenericArguments()[0];
            var method = HandleAsyncMethodInfo.MakeGenericMethod(taskReturnType);
            return (GenericAsyncHandler) method.CreateDelegate(typeof(GenericAsyncHandler));
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsync(invocation, ProceedAsynchronous);
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = InterceptAsync(invocation, ProceedAsynchronous<TResult>);
        }

        protected abstract Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed);

        protected abstract Task<TResult> InterceptAsync<TResult>(
            IInvocation invocation,
            Func<IInvocation, Task<TResult>> proceed);

        private static void InterceptSynchronousVoid(AsyncInterceptor me, IInvocation invocation)
        {
            var task = me.InterceptAsync(invocation, ProceedSynchronous);

            if (!task.IsCompleted) Task.Run(() => task).Wait();

            if (task.IsFaulted) throw task.Exception.InnerException;
        }

        private static void InterceptSynchronousResult<TResult>(AsyncInterceptor me, IInvocation invocation)
        {
            Task task = me.InterceptAsync(invocation, ProceedSynchronous<TResult>);

            if (!task.IsCompleted) Task.Run(() => task).Wait();

            if (task.IsFaulted) throw task.Exception.InnerException;
        }

        private static Task ProceedSynchronous(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        private static Task<TResult> ProceedSynchronous<TResult>(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
                return Task.FromResult((TResult) invocation.ReturnValue);
            }
            catch (Exception e)
            {
                return Task.FromException<TResult>(e);
            }
        }

        private static async Task ProceedAsynchronous(IInvocation invocation)
        {
            invocation.Proceed();
            var originalReturnValue = (Task) invocation.ReturnValue;
            await originalReturnValue.ConfigureAwait(false);
        }

        private static async Task<TResult> ProceedAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.Proceed();
            var originalReturnValue = (Task<TResult>) invocation.ReturnValue;

            var result = await originalReturnValue.ConfigureAwait(false);
            return result;
        }

        private delegate void GenericSynchronousHandler(AsyncInterceptor me, IInvocation invocation);

        private delegate void GenericAsyncHandler(IInvocation invocation, AsyncInterceptor asyncInterceptor);

        private enum MethodType
        {
            Synchronous,
            AsyncAction,
            AsyncFunction
        }
    }
}