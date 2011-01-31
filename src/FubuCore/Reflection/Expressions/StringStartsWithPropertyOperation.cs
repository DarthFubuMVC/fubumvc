using System;
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

    public class StringContainsPropertyOperation : CaseInsensitiveStringMethodPropertyOperation
    {
        private static readonly MethodInfo _method =
            ReflectionHelper.GetMethod<string>(s => s.Contains(""));

        public StringContainsPropertyOperation()
            : base(_method)
        {
        }

        public override string Text
        {
            get { return "contains"; }
        }
    }
}