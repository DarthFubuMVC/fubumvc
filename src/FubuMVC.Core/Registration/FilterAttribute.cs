using System;
using System.Reflection;
using System.Runtime.Serialization;
using FubuMVC.Core.Registration.Nodes;
using FubuCore;
using System.Linq;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Can be used to add one-off ActionFilter's to a single method or action class.
    /// Can only be used with classes that have exactly one public method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class FilterAttribute : ModifyChainAttribute
    {
        private readonly Type _filterType;
        private readonly MethodInfo _method;

        public FilterAttribute(Type filterType)
        {
            _filterType = filterType;
            _method = DetermineMethod(filterType);
        }

        public override void Alter(ActionCall call)
        {
            var filter = new ActionFilter(_filterType, _method);
            call.AddBefore(filter);
        }

        public static MethodInfo DetermineMethod(Type type)
        {
            var methods = type.GetMethods().Where(m => m.DeclaringType != typeof (object)).ToList();
            if (methods.Count() != 1)
            {
                throw new InvalidActionFilterTypeException(type);
            }

            return methods.Single();
        }
    }

    [Serializable]
    public class InvalidActionFilterTypeException : Exception
    {
        public InvalidActionFilterTypeException(Type filterType) : base("Type {0} cannot be used.  Only types with one single public method can be used with the [Filter(type)] attribute".ToFormat(filterType.FullName))
        {
        }

        protected InvalidActionFilterTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}