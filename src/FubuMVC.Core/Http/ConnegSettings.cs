using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    [ApplicationLevel]
    public class ConnegSettings
    {
        public readonly IList<ConnegQuerystring> QuerystringParameters =
            new List<ConnegQuerystring>
            {
                new ConnegQuerystring("Format", "JSON", MimeType.Json),
                new ConnegQuerystring("Format", "XML", MimeType.Xml)
            };

        public readonly IList<IMimetypeCorrection> Corrections = new List<IMimetypeCorrection>();

        public void InterpretQuerystring(CurrentMimeType mimeType, ICurrentHttpRequest request)
        {
            var corrected = QuerystringParameters.FirstValue(x => x.Determine(request.QueryString));
            if (corrected.IsNotEmpty())
            {
                mimeType.AcceptTypes = new MimeTypeList(corrected);
            }
        }
    }
}