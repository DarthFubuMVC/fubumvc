using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Caching
{
    public class OutputCachingNode : BehaviorNode
    {
        public OutputCachingNode()
        {
            ResourceHash = ObjectDef.ForType<ResourceHash>();
            Apply<VaryByResource>();
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Cache; }
        }

        public ObjectDef OutputCache { get; set; }
        public ObjectDef ResourceHash { get; set; }

        public ObjectDef Apply<T>() where T : IVaryBy
        {
            var objectDef = ObjectDef.ForType<T>();
            ResourceHash.EnumerableDependenciesOf<IVaryBy>().Add(objectDef);

            return objectDef;
        }

        public ObjectDef Apply(Type varyByType)
        {
            if (!varyByType.CanBeCastTo<IVaryBy>())
            {
                throw new ArgumentException("varyByType", "varyByType must implement IVaryBy");
            }

            var objectDef = new ObjectDef(varyByType);
            ResourceHash.EnumerableDependenciesOf<IVaryBy>().Add(objectDef);

            return objectDef;
        }

        protected override ObjectDef buildObjectDef()
        {
            var def = ObjectDef.ForType<OutputCachingBehavior>();
            if (OutputCache != null)
            {
                def.Dependency(typeof (IOutputCache), OutputCache);
            }

            if (ResourceHash == null)
            {
                throw new InvalidOperationException("Output caching requires a ResourceHash/VaryBy policy");
            }

            def.Dependency(typeof (IResourceHash), ResourceHash);

            return def;
        }
    }
}