using System;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Validation.Web
{
	public class ValidationActionFilter : ActionFilter, IHaveValidation
	{
		public ValidationActionFilter(Type handlerType, MethodInfo method) : base(handlerType, method)
		{
            Validation = ValidationNode.Default();
		}

        public ValidationNode Validation { get; private set; }

		public static ActionFilter ValidationFor<T>(Expression<Func<T, object>> method)
		{
			return new ValidationActionFilter(typeof(T), ReflectionHelper.GetMethod<T>(method));
		}
	}

    public class ValidationActionFilter<T>
    {
        private readonly IValidationFilter<T> _filter;
        private readonly IFubuRequest _request;

        public ValidationActionFilter(IValidationFilter<T> filter, IFubuRequest request)
        {
            _filter = filter;
            _request = request;
        }

        public FubuContinuation Validate(T input)
        {
            var notification = _filter.Validate(input);
            if(notification.IsValid())
            {
                return FubuContinuation.NextBehavior();
            }

            _request.Set(notification);
            return FubuContinuation.TransferTo(input, categoryOrHttpMethod: "GET");
        }
    }
}