using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using System.Linq;

namespace FubuMVC.Core.Caching
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CacheAttribute : ModifyChainAttribute
    {
        private Type[] _varyBy;

        public Type[] VaryBy
        {
            get { return _varyBy; }
            set
            {
                if (!value.Any())
                {
                    throw new ArgumentNullException("VaryBy", "At least one VaryBy policy is required");
                }

                _varyBy = value;
            }
        }

        public override void Alter(ActionCall call)
        {
            var node = new OutputCachingNode();
            if (_varyBy != null)
            {
                node.ReplaceVaryByRules(_varyBy);
            }

            call.AddToEnd(node);
        }
    }
}