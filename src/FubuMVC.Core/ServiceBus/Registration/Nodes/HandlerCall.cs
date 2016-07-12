using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using StructureMap.Pipeline;

namespace FubuMVC.Core.ServiceBus.Registration.Nodes
{
    public class HandlerCall : ActionCallBase, IMayHaveInputType
    {
        public static Task CascadeAsync<T>(Task<T> task, IInvocationContext context)
        {
            return task.ContinueWith(t => context.EnqueueCascading(t.Result), TaskContinuationOptions.NotOnFaulted);
        }

        public static bool IsCandidate(MethodInfo method)
        {
            if (method.DeclaringType.Equals(typeof(object))) return false;

            var parameterCount = method.GetParameters() == null ? 0 : method.GetParameters().Length;
            if (parameterCount != 1) return false;

            if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
                return false;

            bool hasOutput = method.ReturnType != typeof(void);
            return !hasOutput || !method.ReturnType.IsValueType;
        }

        public static HandlerCall For(Type openType, Type closedType, string methodName)
        {
            var fullType = openType.MakeGenericType(closedType);
            var method = fullType.GetMethod(methodName);

            if (method == null) throw new ArgumentException("Could not find method named '{0}' in type {1}".ToFormat(methodName, fullType.GetFullName()));

            return new HandlerCall(fullType, method);
        }

        public static HandlerCall For<T>(Expression<Action<T>> method)
        {
            return new HandlerCall(typeof(T), ReflectionHelper.GetMethod(method));
        }

        public HandlerCall(Type handlerType, MethodInfo method)
            : base(handlerType, method)
        {
            if (!IsCandidate(method))
            {
                throw new ArgumentOutOfRangeException("method", method, "The method has to have exactly one input");
            }
        }

        public HandlerCall Clone()
        {
            return new HandlerCall(HandlerType, Method);
        }

        public override BehaviorCategory Category => BehaviorCategory.Call;

        protected override IConfiguredInstance buildInstance()
        {
            var invocationType = typeof(HandlerInvocation<>).MakeGenericType(HandlerType);
            var invocation = Activator.CreateInstance(invocationType, Method);

            var instance = new ConfiguredInstance(typeof(HandlerInvoker<>), HandlerType);
            instance.Dependencies.Add(invocationType, invocation);

            return instance;
        }

        public bool CouldHandleOtherMessageType(Type inputType)
        {
            if (inputType == InputType()) return false;

            return inputType.CanBeCastTo(InputType());
        }

        public void AddClone(HandlerChain chain)
        {
            chain.AddToEnd(Clone());
        }

        public override string ToString()
        {
            return "Handler: " + Description;
        }

        public bool Equals(HandlerCall other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.HandlerType == HandlerType && other.Method.Matches(Method);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(HandlerCall)) return false;
            return Equals((HandlerCall)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HandlerType != null ? HandlerType.GetHashCode() : 0) * 397) ^
                       (Method != null ? Method.GetHashCode() : 0);
            }
        }


    }

    public class HandlerInvocation<THandler>
    {
        private static MethodInfo FubuRequestFind = typeof(IFubuRequest).GetMethod(nameof(IFubuRequest.Find));

        private static MethodInfo CascadeAsync = typeof(HandlerCall).GetMethod(nameof(HandlerCall.CascadeAsync),
            BindingFlags.Public | BindingFlags.Static);

        private static MethodInfo EnqueueCascading =
            typeof(IInvocationContext).GetMethod(nameof(IInvocationContext.EnqueueCascading));

        private static MethodInfo FirstOrDefault =
            typeof(Enumerable)
                .GetMethods().FirstOrDefault(x => x.Name == nameof(Enumerable.FirstOrDefault) && x.GetParameters().Length == 1);

        private readonly Func<THandler, IFubuRequest, IInvocationContext, Task> _action;



        public HandlerInvocation(MethodInfo method)
        {
            var request = Expression.Parameter(typeof(IFubuRequest), "request");
            var context = Expression.Parameter(typeof(IInvocationContext), "context");
            var handler = Expression.Parameter(typeof(THandler), "handler");

            var inputs = method.GetParameters().Select(param => buildExpressionForParameter(request, param));

            Expression body = Expression.Call(handler, method, inputs);

            if (method.ReturnType != typeof(Task) && method.ReturnType.CanBeCastTo<Task>())
            {
                var outputType = method.ReturnType.GetGenericArguments().Single();
                var cascade = CascadeAsync.MakeGenericMethod(outputType);
                body = Expression.Call(null, cascade, body, context);
            }
            else if (method.ReturnType != typeof(void) && method.ReturnType != typeof(Task))
            {
                body = Expression.Call(context, EnqueueCascading, body);
            }

            if (!body.Type.CanBeCastTo<Task>())
            {
                var returnStatement = Expression.Constant(Task.CompletedTask);
                body = Expression.Block(body, returnStatement);
            }

            var lambda = Expression
                .Lambda<Func<THandler, IFubuRequest, IInvocationContext, Task>>(body, handler, request, context);

            _action = lambda.Compile();
        }

        private Expression buildExpressionForParameter(ParameterExpression request, ParameterInfo parameter)
        {
            var method = FubuRequestFind.MakeGenericMethod(parameter.ParameterType);

            var inner =  Expression.Call(request, method);

            var firstMethod = FirstOrDefault.MakeGenericMethod(parameter.ParameterType);

            return Expression.Call(null, firstMethod, inner);
        }

        public Task Invoke(THandler handler, IFubuRequest request, IInvocationContext context)
        {
            return _action(handler, request, context);
        }
    }

    public class HandlerInvoker<THandler> : BasicBehavior
    {
        private readonly IFubuRequest _request;
        private readonly THandler _handler;
        private readonly IInvocationContext _context;
        private readonly HandlerInvocation<THandler> _invocation;

        public HandlerInvoker(IFubuRequest request, THandler handler, IInvocationContext context, HandlerInvocation<THandler> invocation) : base(PartialBehavior.Executes)
        {
            _request = request;
            _handler = handler;
            _context = context;
            _invocation = invocation;
        }

        protected override async Task<DoNext> performInvoke()
        {
            await _invocation.Invoke(_handler, _request, _context).ConfigureAwait(false);

            return DoNext.Continue;
        }
    }
}