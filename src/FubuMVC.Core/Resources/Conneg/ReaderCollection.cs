using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg
{
    [Description("Smart collection of conneg readers")]
    public class ReaderCollection<T> : IReaderCollection<T> where T : class
    {
        private readonly IList<IReader<T>> _readers = new List<IReader<T>>();

        public ReaderCollection(IEnumerable<IReader<T>> readers)
        {
            _readers.AddRange(readers);
        }

        public ReaderCollection(IEnumerable<IReader> readers)
            : this(readers.OfType<IReader<T>>().ToArray())
        {
        }

        public void Add(IReader<T> reader)
        {
            _readers.Add(reader);
        }

        public IReader<T> ChooseReader(CurrentMimeType mimeTypes, IFubuRequestContext context)
        {
            // TODO -- VERY TEMPORARY
            if (!_readers.Any())
            {
                _readers.Add(new ModelBindingReader<T>());
                _readers.Add(new FormatterReader<T>(new JsonSerializer()));
            }

            return _readers.FirstOrDefault(x => x.Mimetypes.Contains(mimeTypes.ContentType));
        }

        public IEnumerable<IReader<T>> Readers
        {
            get
            {
                return _readers;
            }
        }
    }
}