using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media.Formatters
{
    public class FormatterMediaWriter<T> : IMediaWriter<T>
    {
        private readonly IEnumerable<IFormatter> _formatters;
        private readonly IFubuRequest _request;

        public FormatterMediaWriter(IFubuRequest request, IEnumerable<IFormatter> formatters)
        {
            if (!formatters.Any())
            {
                throw new ArgumentException("Must be at least one formatter");
            }

            _request = request;
            _formatters = formatters;
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _formatters.SelectMany(x => x.MatchingMimetypes); }
        }

        public void Write(IValues<T> source, IOutputWriter writer)
        {
            Write(source.Subject, writer);
        }

        public void Write(T source, IOutputWriter writer)
        {
            Write(source);
        }

        public void Write(T source)
        {
            var currentMimeTypes = _request.Get<CurrentMimeType>();
            foreach (var acceptType in currentMimeTypes.AcceptTypes)
            {
                var formatter = _formatters.FirstOrDefault(x => x.MatchingMimetypes.Contains(acceptType));
                if (formatter != null)
                {
                    formatter.Write(source, acceptType);
                    return;
                }
            }

            var firstFormatter = _formatters.First();
            firstFormatter.Write(source, firstFormatter.MatchingMimetypes.First());
        }
    }
}