using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Caching
{
    public class RequestOutputCacheNode<TInputModel> : BehaviorNode where TInputModel : class
    {
        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Cache; }
        }

        protected override ObjectDef buildObjectDef()
        {
            var objectDef = new ObjectDef(typeof(RequestOutputCacheBehavior<TInputModel>));

            return objectDef;
        }
    }
}