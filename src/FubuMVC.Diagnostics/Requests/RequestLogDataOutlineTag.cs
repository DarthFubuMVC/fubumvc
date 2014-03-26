using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuMVC.Diagnostics.Runtime;

namespace FubuMVC.Diagnostics.Requests
{
    public class RequestLogDataOutlineTag : OutlineTag
    {
        public RequestLogDataOutlineTag(RequestLog log, BehaviorChain chain)
        {
            AddHeader("Data");

            log.RequestData.Reports.Where(x => x.Values.Any()).Each(
                report =>
                {
                    AddNode(report.Header(), report.ElementId());
                });


            if (log.ResponseHeaders.Any())
            {
                AddNode(ResponseHeadersTag.Heading, ResponseHeadersTag.ElementId);
            }
        }
    }
}