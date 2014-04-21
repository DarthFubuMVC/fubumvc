using System;
using System.Linq;

namespace FubuMVC.Core.Caching
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CacheAttribute : Attribute
    {
        private Type[] _varyBy;

        public CacheAttribute(params Type[] varyBy)
        {
            _varyBy = varyBy;
        }

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

        public void Alter(OutputCachingNode cachingNode)
        {
            if (_varyBy != null && _varyBy.Any())
            {
                cachingNode.ReplaceVaryByRules(_varyBy);
            }
        }
    }
}