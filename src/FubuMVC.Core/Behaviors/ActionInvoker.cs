using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public static class TaskExtensions
    {
        public static Task SetFubuRequestValue<T>(Task<T> task, IFubuRequest request) where T : class
        {
            return task.ContinueWith(t => request.Set<T>(t.Result), TaskContinuationOptions.NotOnFaulted);
        }
    }

    public class ActionInvocation<TEndpoint>
    {


        private static readonly MethodInfo FubuRequestSet = typeof(IFubuRequest).GetMethod(nameof(IFubuRequest.Set),
            new Type[] {typeof(Type), typeof(object)});

        private readonly Func<TEndpoint, IFubuRequest, Task> _action;

        private static readonly MethodInfo SetFubuRequestValue =
            typeof(TaskExtensions).GetMethod(nameof(TaskExtensions.SetFubuRequestValue), BindingFlags.Static | BindingFlags.Public);

        
        public ActionInvocation(MethodInfo method)
        {
            var request = Expression.Parameter(typeof(IFubuRequest), "request");
            var endpoint = Expression.Parameter(typeof(TEndpoint), "endpoint");

            var inputs = method.GetParameters().Select(param => buildExpressionForParameter(request, param));

            Expression body = Expression.Call(endpoint, method, inputs);

            if (method.ReturnType != typeof(Task) && method.ReturnType.CanBeCastTo<Task>())
            {
                var outputType = method.ReturnType.GetGenericArguments().Single();

                var setRequestMethod = SetFubuRequestValue.MakeGenericMethod(outputType);
                body = Expression.Call(null, setRequestMethod, body, request);

            }
            else if (method.ReturnType != typeof(void) && method.ReturnType != typeof(Task))
            {
                body = setRequestExpression(method.ReturnType, request, body);
            }


            if (!body.Type.CanBeCastTo<Task>())
            {
                var returnStatement = Expression.Constant(Task.CompletedTask);
                body = Expression.Block(body, returnStatement);
            }


            var lambda = Expression.Lambda<Func<TEndpoint, IFubuRequest, Task>>(body, endpoint, request);

            _action = lambda.Compile();
        }

        private Expression setRequestExpression(Type outputType, ParameterExpression request, Expression value)
        {
            return Expression.Call(request, FubuRequestSet, Expression.Constant(outputType), value);
        }

        private Expression buildExpressionForParameter(ParameterExpression request, ParameterInfo parameter)
        {
            var method = typeof(IFubuRequest).GetMethod(nameof(IFubuRequest.Get), new Type[0]).MakeGenericMethod(parameter.ParameterType);

            return Expression.Call(request, method);
        }

        public Task Invoke(IFubuRequest request, TEndpoint endpoint)
        {
            return _action(endpoint, request);
        }
    }


    public class ActionInvoker<TEndpoint> : BasicBehavior
    {
        private readonly IFubuRequest _request;
        private readonly TEndpoint _endpoint;
        private readonly ActionInvocation<TEndpoint> _invocation;

        public ActionInvoker(IFubuRequest request, TEndpoint endpoint, ActionInvocation<TEndpoint> invocation) : base(PartialBehavior.Executes)
        {
            _request = request;
            _endpoint = endpoint;
            _invocation = invocation;
        }

        protected override async Task<DoNext> performInvoke()
        {
            await _invocation.Invoke(_request, _endpoint).ConfigureAwait(false);

            return DoNext.Continue;
        }
    }
}