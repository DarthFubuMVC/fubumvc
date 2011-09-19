using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Rest.Projections
{
    // TODO -- might want something besides IDisplayFormatter
    public class FormattedValueSource<T> : IValueSource<T>
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IValueSource<T> _inner;

        public FormattedValueSource(IDisplayFormatter formatter, IValueSource<T> inner)
        {
            _formatter = formatter;
            _inner = inner;
        }

        public object ValueFor(Accessor accessor)
        {
            var innerValue = _inner.ValueFor(accessor);
            return _formatter.GetDisplay(accessor, innerValue);
        }

        public T Subject
        {
            get { return _inner.Subject; }
        }
    }
}