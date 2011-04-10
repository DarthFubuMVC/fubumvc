using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Conneg
{
    public class MediaProcessor<T> : IMediaProcessor<T>
    {
        private readonly IEnumerable<IFormatter> _formatters;

        public MediaProcessor(IEnumerable<IFormatter> formatters)
        {
            _formatters = formatters;
        }

        public T Retrieve(CurrentRequest request)
        {
            return findFormatter(request).Read<T>(request);
        }

        private IFormatter findFormatter(CurrentRequest request)
        {
            var formatter = _formatters.FirstOrDefault(x => x.Matches(request));
            if (formatter == null)
            {
                throw new MediaProcessingException("Could not determine a formatter for this request");
            }

            return formatter;
        }

        public void Write(T target, CurrentRequest request)
        {
            findFormatter(request).Write(target, request);
        }
    }
}