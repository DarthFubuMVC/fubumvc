using System;
using System.Collections.Generic;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Runtime;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    public class RequestTable : TableTag
    {
        public RequestTable(IUrlRegistry urls, IEnumerable<RequestLog> logs)
        {
            AddClass("table");

            AddHeaderRow(row => {
                row.Header("Time (Local)");
                row.Header("Endpoint");
                row.Header("Method");
                row.Header("Status");
                row.Header("Content Type");
                row.Header("Duration (ms)");
            });

            logs.Each(log => writeLog(log, urls));
        }

        private void writeLog(RequestLog log, IUrlRegistry urls)
        {
            AddBodyRow(row => {
                var chainUrl = urls.UrlFor(log);
                row.Cell().Add("a").Text(log.LocalTime).Attr("href", chainUrl);
                row.Cell(log.Endpoint);
                row.Cell(log.HttpMethod);
                var statusCell = row.Cell();
                statusCell.Add("span").AddClass("http-status-code").Text(log.HttpStatus.Status.ToString());
                statusCell.Add("span").AddClass("http-status-description").Text(log.HttpStatus.Description);
                
                row.Cell(log.ContentType);
                row.Cell(Math.Ceiling(log.ExecutionTime).ToString()).AddClass("number");
            });
        }
    }
}