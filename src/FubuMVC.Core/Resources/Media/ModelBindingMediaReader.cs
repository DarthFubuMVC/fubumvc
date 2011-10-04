using System.Collections.Generic;
using FubuCore.Binding;

namespace FubuMVC.Core.Resources.Media
{
    public class ModelBindingMediaReader<T> : IMediaReader<T>
    {
        // TODO -- move to MimeTypes when Assets is put in place
        public static readonly string HttpFormMimetype = "application/x-www-form-urlencoded";

        private readonly IBindingContext _context;
        private readonly IObjectResolver _resolver;

        public ModelBindingMediaReader(IObjectResolver resolver, IBindingContext context)
        {
            _resolver = resolver;
            _context = context;
        }

        public T Read(string mimeType)
        {
            var result = _resolver.BindModel(typeof (T), _context);
            return (T) result.Value;
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return HttpFormMimetype; }
        }
    }
}