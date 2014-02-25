using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Resources.Conneg
{
    [Description("Smart collection of conneg readers")]
    public class ReaderCollection<T> : IReaderCollection<T> where T : class
    {
        private readonly IInputNode _node;

        public ReaderCollection(IInputNode node)
        {
            _node = node;
        }

        public void Add(IReader<T> reader)
        {
            _node.Add(reader);
        }

        public IReader<T> ChooseReader(CurrentMimeType mimeTypes, IFubuRequestContext context)
        {
            return _node.Readers().OfType<IReader<T>>().FirstOrDefault(x => x.Mimetypes.Contains(mimeTypes.ContentType));
        }
    }
}