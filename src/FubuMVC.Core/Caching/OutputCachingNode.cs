using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Etags;

namespace FubuMVC.Core.Caching
{
    public class OutputCachingNode : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Cache; }
        }

        public ObjectDef OutputCache { get; set; }
        public ObjectDef ETagCache { get; set; }

        protected override ObjectDef buildObjectDef()
        {
            var def = ObjectDef.ForType<OutputCachingBehavior>();
            if (OutputCache != null)
            {
                def.Dependency(typeof (IOutputCache), OutputCache);
            }

            if (ETagCache != null)
            {
                def.Dependency(typeof (IEtagCache), ETagCache);
            }

            return def;
        }
    }
}