using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using System.Linq;

namespace FubuMVC.Core.Caching
{
    [Description("Caches the output of the following nodes in the chain")]
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
            return Apply(typeof (T));
        }

        public ObjectDef Apply(Type varyByType)
        {
            if (VaryByPolicies().Contains(varyByType)) return null;

            if (!varyByType.CanBeCastTo<IVaryBy>())
            {
                throw new ArgumentException("varyByType", "varyByType must implement IVaryBy");
            }

            var objectDef = new ObjectDef(varyByType);
            ResourceHash.EnumerableDependenciesOf<IVaryBy>().Add(objectDef);

            return objectDef;
        }

        public IEnumerable<Type> VaryByPolicies()
        {
            foreach (var objectDef in ResourceHash.EnumerableDependenciesOf<IVaryBy>().Items)
            {
                if (objectDef.Type != null)
                {
                    yield return objectDef.Type;
                }

                if (objectDef.Value != null )
                {
                    yield return objectDef.Value.GetType();
                }
            }
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

        public void ReplaceVaryByRules(Type[] varyBy)
        {
            ResourceHash.EnumerableDependenciesOf<IVaryBy>().Clear();

            varyBy.Each(t => Apply(t));
        }
    }
}