using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuCore.Reflection.Expressions
{
    public abstract class CaseInsensitiveStringMethodPropertyOperation : IPropertyOperation
    {
        private readonly MethodInfo _method;
        private readonly bool _negate;

        protected CaseInsensitiveStringMethodPropertyOperation(MethodInfo method) : this(method, false) { }

        protected CaseInsensitiveStringMethodPropertyOperation(MethodInfo method, bool negate)
        {
            _method = method;
            _negate = negate;
        }

        public virtual string OperationName
        {
            get { return _method.Name; }
        }
        public abstract string Text { get; }

        public Func<object, Expression<Func<ENTITY, bool>>> GetPredicateBuilder<ENTITY>(MemberExpression propertyPath)
        {
            return valueToCheck =>
            {
                ConstantExpression valueToCheckConstant = Expression.Constant(valueToCheck);
                Expression expression =
                    Expression.Call(Expression.Coalesce(propertyPath, Expression.Constant(string.Empty)), _method,
                                    valueToCheckConstant,
                                    Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
                if (_negate)
                {
                    expression = Expression.Not(expression);
                }

                ParameterExpression lambdaParameter = propertyPath.GetParameter<ENTITY>();
                return Expression.Lambda<Func<ENTITY, bool>>(expression, lambdaParameter);
            };
        }
    }
}