using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public class FilterAdded : NodeEvent, DescribesItself
    {
        private readonly IBehaviorInvocationFilter _filter;

        public FilterAdded(IBehaviorInvocationFilter filter)
        {
            _filter = filter;
        }

        public void Describe(Description description)
        {
            description.Title = "Chain Filter Added";
            description.AddChild("Filter", _filter);
        }

        protected bool Equals(FilterAdded other)
        {
            return Equals(_filter, other._filter);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FilterAdded) obj);
        }

        public override int GetHashCode()
        {
            return (_filter != null ? _filter.GetHashCode() : 0);
        }
    }
}