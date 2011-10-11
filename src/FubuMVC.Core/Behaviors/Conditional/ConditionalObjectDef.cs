using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public static class ConditionalObjectDef
    {
        public static ObjectDef For<T>() where T : IConditional
        {
            return new ObjectDef(typeof(T));
        }

        public static ObjectDef ForModel<T>(Func<T, bool> filter) where T : class
        {
            var def = new ObjectDef(typeof (LambdaConditional<>), typeof (IFubuRequest));
            Func<IFubuRequest, bool> match = request => filter(request.Get<T>());
            def.DependencyByValue(match);

            return def;
        }

        public static ObjectDef ForService<T>(Func<T, bool> filter)
        {
            var def = new ObjectDef(typeof(LambdaConditional<>), typeof(T));
            def.DependencyByValue(filter);

            return def;
        }

        public static ObjectDef For(Func<bool> filter)
        {
            var def = new ObjectDef(typeof (LambdaConditional));
            def.DependencyByValue(filter);

            return def;
        }
    }
}