using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.OwinHost.Readers
{
    public class MediaTypeReader : IOwinRequestReader
    {
        public void Read(IDictionary<string, object> environment)
        {
            var headers = environment.Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey);
            var contentType = headers.Get(OwinConstants.ContentTypeHeader).First();

            var commaSemicolon = new[] { ',', ';' };
            var delimiterPos = contentType.IndexOfAny(commaSemicolon);
            var mediaType = delimiterPos < 0 ? contentType : contentType.Substring(0, delimiterPos);
            environment.Add(OwinConstants.MediaTypeKey, mediaType);
        }
    }
}