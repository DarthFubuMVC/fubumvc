using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Core.UI.Bootstrap.Tags;

namespace FubuMVC.Diagnostics.Requests
{
    public class ResponseHeadersTag : CollapsibleTag
    {
        public static readonly string ElementId = "response-headers";
        public static readonly string Heading = "Response Headers";

        public ResponseHeadersTag(RequestLog log) : base(ElementId, Heading)
        {
            if (!log.ResponseHeaders.Any())
            {
                Render(false);
                return;
            }

            var detailsTag = new DetailsTableTag();
            log.ResponseHeaders.OrderBy(x => x.Name).Each(header => detailsTag.AddDetail(header.Name, header.Value));

            AppendContent(detailsTag);
        }
    }
}