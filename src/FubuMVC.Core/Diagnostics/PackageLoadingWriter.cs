using System.ComponentModel;
using System.Reflection;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Urls;
using HtmlTags;
using FubuCore;

namespace FubuMVC.Core.Diagnostics
{
    [FubuDiagnostics("Package Loading and Bootstrapping")]
    public class PackageLoadingWriter
    {
        private readonly IUrlRegistry _urls;

        public PackageLoadingWriter(IUrlRegistry urls)
        {
            _urls = urls;
        }

        [FubuDiagnostics("Package Loading and Bootstrapping")]
        public HtmlDocument FullLog()
        {
            
            var table = new TableTag();
            table.AddHeaderRow(r =>
            {
                r.Header("Type");
                r.Header("Description");
                r.Header("Provenance");
                r.Header("Timing");
            });

            PackageRegistry.Diagnostics.EachLog((target, log) =>
            {
                table.AddBodyRow(row =>
                {
                    row.Cell(PackagingDiagnostics.GetTypeName(target));
                    row.Cell(target.ToString());
                    row.Cell(log.Provenance);
                    row.Cell(log.TimeInMilliseconds.ToString()).AddClass("execution-time");
                });

                if (log.FullTraceText().IsNotEmpty())
                {
                    table.AddBodyRow(row =>
                    {
                        row.Cell().Attr("colspan", "4").Add("pre").AddClass("log").Text(log.FullTraceText());
                        if (!log.Success)
                        {
                            row.AddClass("failure");
                        }
                    });
                }
            });

            var document = DiagnosticHtml.BuildDocument(_urls, "Full Package Loading Log", table);



            return document;
        }


    }
}