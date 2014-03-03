using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Owin.Readers
{
    public class MediaTypeReader : IOwinRequestReader
    {
        public void Read(IDictionary<string, object> environment)
        {
            var headers = environment.Get<IDictionary<string, string[]>>(OwinConstants.RequestHeadersKey);
	        var values = headers.Get(OwinConstants.ContentTypeHeader);
	        var contentType = MimeType.Html.Value;

			if (values != null && values.Any())
			{
				contentType = values.First();
			}

            var commaSemicolon = new[] { ',', ';' };
            var delimiterPos = contentType.IndexOfAny(commaSemicolon);
            var mediaType = delimiterPos < 0 ? contentType : contentType.Substring(0, delimiterPos);
            environment.Add(OwinConstants.MediaTypeKey, mediaType);
        }
    }
}