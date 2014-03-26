
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding.Values;
using FubuMVC.Core.UI.Bootstrap.Collapsibles;
using FubuMVC.Core.UI.Bootstrap.Tags;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    public class RequestDataFubuDiagnostics
    {
        public HtmlTag RequestDataPartial(ValueReport report)
        {
            var tag = new HtmlTag("div").Id("request-data");

            var dataTags = report.Reports.Where(x => x.Values.Any()).Select(toTag);

            tag.Append(dataTags);

            return tag;
        }

        private static HtmlTag toTag(ValueSourceReport report)
        {
            var table = new DetailsTableTag();

            foreach (var key in report.Values.GetAllKeys().OrderBy(x => x))
            {
                table.AddDetail(key, string.Join("; ", report.Values[key]));
            }

            var tag = new CollapsibleTag(report.ElementId(), report.Header());
            tag.AppendContent(table);

            return tag.PrependAnchor();
        }
    }

    public static class ValueSourceReportExtensions
    {
        public static string Header(this ValueSourceReport report)
        {
            return report.Name.SplitPascalCase();
        }

        public static string ElementId(this ValueSourceReport report)
        {
            return "request-data-" + report.Name;
        }
    }
}