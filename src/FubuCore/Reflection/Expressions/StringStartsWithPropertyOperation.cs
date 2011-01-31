using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuCore.Reflection.Expressions
{
    public class StringStartsWithPropertyOperation : CaseInsensitiveStringMethodPropertyOperation
    {
        private static readonly MethodInfo _method =
            ReflectionHelper.GetMethod<string>(s => s.StartsWith("", StringComparison.CurrentCulture));

        public StringStartsWithPropertyOperation()
            : base(_method)
        {
        }

        public override string Text
        {
            get { return "starts with"; }
        }
    }

    public class StringContainsPropertyOperation : StringContainsPropertyOperationBase
    {
        public StringContainsPropertyOperation() : base("Contains", "contains", false)
        {
        }
    }

    public class StringDoesNotContainPropertyOperation : StringContainsPropertyOperationBase
    {
        public StringDoesNotContainPropertyOperation() : base("DoesNotContain", "does not contain", true)
        {
        }
    }

    public abstract class StringContainsPropertyOperationBase : IPropertyOperation
    {
        private static readonly MethodInfo _indexOfMethod;
        private readonly string _operation;
        private readonly string _description;
        private readonly bool _negate;

        static StringContainsPropertyOperationBase()
        {
            _indexOfMethod =
                ReflectionHelper.GetMethod<string>(s => s.IndexOf("", StringComparison.InvariantCultureIgnoreCase));
        }

        protected StringContainsPropertyOperationBase(string operation, string description, bool negate)
        {
            _operation = operation;
            _description = description;
            _negate = negate;
        }

        public string OperationName { get { return _operation; } }
        public string Text
        {
            get { return _description; }
        }

        public Func<object, Expression<Func<T, bool>>> GetPredicateBuilder<T>(MemberExpression propertyPath)
        {
            return valueToCheck =>
            {
                ConstantExpression valueToCheckConstant = Expression.Constant(valueToCheck);
                MethodCallExpression indexOfCall =
                    Expression.Call(Expression.Coalesce(propertyPath, Expression.Constant(String.Empty)), _indexOfMethod,
                                    valueToCheckConstant,
                                    Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
                var operation = _negate ? ExpressionType.LessThan : ExpressionType.GreaterThanOrEqual;
                BinaryExpression comparison = Expression.MakeBinary(operation, indexOfCall,
                                                                    Expression.Constant(0));
                ParameterExpression lambdaParameter = propertyPath.GetParameter<T>();
                return Expression.Lambda<Func<T, bool>>(comparison, lambdaParameter);
            };
        }
    }
}