using System;
using System.ComponentModel;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.AntiForgery
{
    [Description("Applies XSS anti forgery checks against this BehaviorChain")]
    public class AntiForgeryNode : BehaviorNode
    {
        private readonly string _salt;

        public AntiForgeryNode(string salt)
        {
            if (salt == null) throw new ArgumentNullException("salt");

            _salt = salt;
        }

        public override BehaviorCategory Category
        {
            get { return BehaviorCategory.Wrapper; }
        }

        protected override ObjectDef buildObjectDef()
        {
            return ObjectDef.ForType<AntiForgeryBehavior>(x => x.DependencyByValue(_salt));
        }

        public string Salt
        {
            get { return _salt; }
        }

        public bool Equals(AntiForgeryNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._salt, _salt);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AntiForgeryNode)) return false;
            return Equals((AntiForgeryNode) obj);
        }

        public override int GetHashCode()
        {
            return (_salt != null ? _salt.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Salt: {0}", _salt);
        }
    }
}