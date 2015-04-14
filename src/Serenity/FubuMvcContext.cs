using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using FubuCore;
using FubuMVC.Core.Diagnostics.Runtime;
using HtmlTags;
using StoryTeller;
using StoryTeller.Engine;
using StoryTeller.Results;

namespace Serenity
{
    public class FubuMvcContext : IExecutionContext
    {
        private readonly FubuMvcSystem _system;

        public FubuMvcContext(FubuMvcSystem system)
        {
            _system = system;
        }

        public void Dispose()
        {
        }

        public IServiceLocator Services
        {
            get { return _system.Application.Services; }
        }

        public void AfterExecution(ISpecContext context)
        {
            var reporter = context.Reporting.ReporterFor<RequestReporter>();
            reporter.Append(Services.GetInstance<IRequestHistoryCache>().RecentReports().ToArray());
        }
    }

    public class RequestReporter : IReporter
    {
        private readonly List<RequestLog> _logs = new List<RequestLog>();



        public HtmlTag ToHtml()
        {
            var table = new TableTag();
            table.AddClass("table");
            table.AddClass("table-striped");
            table.AddHeaderRow(row =>
            {
                row.Header("Details");
                row.Header("Duration (ms)");
                row.Header("Method");
                row.Header("Endpoint");
                row.Header("Status");
                row.Header("Content Type");
            });

            _logs.Each(log =>
            {
                var url = "/_fubu#/fubumvc/request-details/" + log.Id;

                table.AddBodyRow(row =>
                {
                    row.Cell().Add("a").Text("Details").Attr("href", url);
                    row.Cell(log.ExecutionTime.ToString()).Attr("align", "right");
                    row.Cell(log.HttpMethod);
                    row.Cell(log.Endpoint);
                    row.Cell(log.StatusCode.ToString());
                    row.Cell(log.ContentType);
                });
            });



            return table;
        }

        public string Title
        {
            get { return "FubuMVC Requests During the Specification Execution"; }
            
        }

        public void Append(RequestLog[] requests)
        {
            _logs.AddRange(requests);
        }
    }
}