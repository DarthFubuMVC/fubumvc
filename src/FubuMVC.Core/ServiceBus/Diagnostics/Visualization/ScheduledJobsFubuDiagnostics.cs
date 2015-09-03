using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Execution;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics.Visualization
{
    public class ScheduledJobsFubuDiagnostics
    {
        private readonly IUrlRegistry _urls;
        private readonly ISchedulePersistence _persistence;
        private readonly ChannelGraph _graph;
        private readonly ScheduledJobGraph _jobs;
        private readonly IScheduledJobController _controller;

        public ScheduledJobsFubuDiagnostics(IUrlRegistry urls, ISchedulePersistence persistence, ChannelGraph graph, ScheduledJobGraph jobs, IScheduledJobController controller)
        {
            _urls = urls;
            _persistence = persistence;
            _graph = graph;
            _jobs = jobs;
            _controller = controller;
        }

        [System.ComponentModel.Description("Schedules:Scheduled Job Monitor")]
        public HtmlTag get_scheduled_jobs()
        {
            var tag = new HtmlTag("div");
            tag.Add("h1").Text("Scheduled Jobs Monitor");
            tag.Add("p").Text("at {0} -- reload the page to refresh the data".ToFormat(DateTime.Now));

            var schedule = _persistence.FindAll(_graph.Name);
            tag.Append(new ScheduledJobTable(_urls, schedule));

            return tag;
        }

        [System.ComponentModel.Description("Scheduled Job History")]
        public HtmlTag get_job_details_Job(ScheduledJobRequest request)
        {
            var schedule = _persistence.Find(_graph.Name, request.Job);

            var tag = new HtmlTag("div");
            tag.Add("h2").Text("Recent Execution History for " + request.Job);

            var job = _jobs.Jobs.FirstOrDefault(x => JobStatus.GetKey(x.JobType) == request.Job);
            if (job != null)
            {
                var descriptionTag = new DescriptionBodyTag(Description.For(job));
                tag.Append(descriptionTag);
                tag.Append("hr");
                tag.Append("h4").Text("History");
            }

            var history = _persistence.FindHistory(_graph.Name, request.Job);
            tag.Append(new ScheduledJobHistoryTable(history));

            return tag;
        }

        public FubuContinuation get_jobs_execute_Name(RunJobRequest request)
        {
            var job = _jobs.Jobs.FirstOrDefault(x => x.JobType.Name.EqualsIgnoreCase(request.Name));
            if (job != null)
            {
                _controller.ExecuteNow(job);
            }

            return FubuContinuation.RedirectTo<ScheduledJobsFubuDiagnostics>(_ => _.get_scheduled_jobs());
        }
    }

    public class RunJobRequest
    {
        public string Name { get; set; }
    }

    public class ScheduledJobRequest
    {
        public string Job { get; set; }
    }

    public class ScheduledJobHistoryTable : TableTag
    {
        public readonly string Script = @"

";

        public ScheduledJobHistoryTable(IEnumerable<JobExecutionRecord> records)
        {
            AddClass("table");

            AddHeaderRow(row => {
                row.Header("Finished");
                row.Header("Node");
                row.Header("Success");
                row.Header("Duration (ms)");
                row.Header("Attempt #");
                row.Header("Exception");
            });

            records.OrderByDescending(x => x.Finished).Each(record => {
                AddBodyRow(row => addRecord(record, row));
            });  
        }

        private void addRecord(JobExecutionRecord record, TableRowTag row)
        {
            row.Cell(record.Finished.ToLocalTime().ToString()).Style("vertical-align", "top");
            row.Cell(record.Executor).Style("vertical-align", "top");
            row.Cell(record.Success ? "Success" : "Failed").Style("vertical-align", "top");
            row.Cell(record.Duration.ToString()).Attr("align", "right").Style("vertical-align", "top");
            row.Cell(record.Attempts.ToString()).Attr("align", "right").Style("vertical-align", "top");
            if (record.ExceptionText.IsEmpty())
            {
                row.Cell("None");
            }
            else
            {
                row.Cell().Add("pre").Text(record.ExceptionText).Style("font-size", "xx-small");
            }
        }
    }

    public class ScheduledJobTable : TableTag
    {
        public ScheduledJobTable(IUrlRegistry urls, IEnumerable<JobStatusDTO> statusList)
        {
            AddClass("table");

            AddHeaderRow(row => {
                row.Header("Job");
                row.Header("Next Time (Estimate)");
                row.Header("Status"); // show on node
                row.Header("Last Execution");
                row.Header("Execute");
            });



            statusList.Each(job => {
                AddBodyRow(row => addJobRow(row, job, urls));
            });
        }

        private void addJobRow(TableRowTag row, JobStatusDTO job, IUrlRegistry urls)
        {
            row.Cell().Add("a").Text(job.JobKey).Attr("href", urls.UrlFor(new ScheduledJobRequest{Job = job.JobKey}));
            row.Cell(job.NextTime.HasValue ? job.NextTime.Value.ToLocalTime().ToString() : "Not scheduled");
            row.Cell(job.GetStatusDescription());
            row.Cell(job.GetLastExecutionDescription());

            var url = urls.UrlFor(new RunJobRequest {Name = job.JobKey});

            row.Cell().Add("button").Text("Execute").Attr("data-url", url).AddClass("button").AddClass("executor").Attr("onclick", "if (window.confirm('Ok to run this job?')) window.location='" + url + "'");
        }
    }
}