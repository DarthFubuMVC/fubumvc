using System;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public class ConditionalService : IConditionalService
    {
        private readonly Cache<Type, bool> _conditions;

        public ConditionalService(IServiceLocator services)
        {
            _conditions = new Cache<Type, bool>(type =>
            {
                var condition = services.GetInstance(type).As<IConditional>();

                return condition.ShouldExecute();
            });
        }

        public bool IsTrue(Type type)
        {
            if (type == typeof(Always))
            {
                return true;
            }

            if (!type.CanBeCastTo<IConditional>())
            {
                throw new ArgumentOutOfRangeException("Only types that implement IConditional may be used here");
            }

            return _conditions[type];
        }
    }
}