using System.Collections.Generic;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Diagnostics.HtmlWriting.Columns
{
    public class DebugWriter
    {
        private readonly IDebugReport _report;
        private readonly IUrlRegistry _urls;

        public DebugWriter(IDebugReport report, IUrlRegistry urls)
        {
            _report = report;
            _urls = urls;
        }

        public HtmlDocument Write()
        {
            HtmlTag header = new HtmlTag("div", x =>
            {
                var table = x.Add<TableTag>()
                    .AddBodyRow(row =>
                        {
                            row.Cell("Request Url:");
                            row.Cell(_report.Url).AddClass("cell-data");
                        })
                    .AddBodyRow(row =>
                        {
                            row.Cell("Execution Time:");
                            row.Cell(_report.ExecutionTime + " milliseconds");
                        })
                    .AddBodyRow(row =>
                        {
                            row.Cell("At:");
                            row.Cell(_report.Time.ToString("G"));
                        });

                table.AddClass("summary");

                writeFormData(x);
            });

            var reportWriter = new DebugReportTagWriter();
            _report.Steps.Each(reportWriter.WriteStep);

            string title = "Debug Run of " + _report.Url;
            return DiagnosticHtml.BuildDocument(_urls, title, header, reportWriter.Tag);
        }

        private void writeFormData(HtmlTag tag)
        {
            if (_report.FormData.Count == 0) return;

            tag.Add("h4").Text("Form Data");


            var table = tag.Add("div").AddClass("formData").Add<TableTag>();

            table.AddHeaderRow(row =>
            {
                row.Header("Key");
                row.Header("Value");
            });


            _report.FormData.Each(pair =>
            {
                table.AddBodyRow(row =>
                {
                    row.Cell(pair.Key);
                    if (pair.Value != null) row.Cell(pair.Value.ToString());
                });
            });
        }
    }
}