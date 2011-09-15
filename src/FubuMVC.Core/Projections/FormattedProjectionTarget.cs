using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Projections
{
    // Hide IDisplayFormatter here?

    public class FormattedProjectionTarget : IProjectionTarget
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IProjectionTarget _inner;

        public FormattedProjectionTarget(IDisplayFormatter formatter, IProjectionTarget inner)
        {
            _formatter = formatter;
            _inner = inner;
        }

        public object ValueFor(Accessor accessor)
        {
            var innerValue = _inner.ValueFor(accessor);
            return _formatter.GetDisplay(accessor, innerValue);
        }

        public IUrlRegistry Urls
        {
            get { return _inner.Urls; }
        }

        public object Subject
        {
            get { return _inner.Subject; }
        }
    }
}