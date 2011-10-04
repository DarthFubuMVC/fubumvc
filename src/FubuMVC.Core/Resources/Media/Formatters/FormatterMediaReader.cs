using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media.Formatters
{
    public class FormatterMediaReader<T> : IMediaReader<T>
    {
        private readonly ISetterBinder _binder;
        private readonly IEnumerable<IFormatter> _formatters;

        public FormatterMediaReader(IEnumerable<IFormatter> formatters, ISetterBinder binder)
        {
            _formatters = formatters;
            _binder = binder;
        }

        public T Read(string mimeType)
        {
            var formatter = _formatters.First(x => x.MatchingMimetypes.Contains(mimeType));
            var target = formatter.Read<T>();
            _binder.BindProperties(target);

            return target;
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatters.SelectMany(x => x.MatchingMimetypes); }
        }
    }
}