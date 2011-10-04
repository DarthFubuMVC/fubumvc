using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Resources.Media
{
    // TODO -- might want something besides IDisplayFormatter
    public class FormattedValues<T> : IValues<T>
    {
        private readonly IDisplayFormatter _formatter;
        private readonly IValues<T> _inner;

        public FormattedValues(IDisplayFormatter formatter, IValues<T> inner)
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