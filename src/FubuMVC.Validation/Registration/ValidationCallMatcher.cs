using FubuCore.Util;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Validation.Registration
{
    public class ValidationCallMatcher
    {
        private readonly CompositeFilter<ActionCall> _callFilters = new CompositeFilter<ActionCall>();

        public ValidationCallMatcher()
        {
            _callFilters.Excludes += call => !call.HasInput;
        }

        public CompositeFilter<ActionCall> CallFilters { get { return _callFilters; } }
    }
}