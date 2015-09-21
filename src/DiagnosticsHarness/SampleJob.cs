using System;
using System.Threading;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.View;
using HtmlTags;

namespace DiagnosticsHarness
{
    public class SampleJob : IJob
    {
        public static bool WillSucceed = true;
        public static int Delay = 2;

        public static int ExecutionCount = 0;

        public void Execute(CancellationToken cancellation)
        {
            ExecutionCount++;

            if (!WillSucceed) throw new DivideByZeroException("You cannot call me");

            Thread.Sleep(2.Seconds());
        }
    }

    public class SampleJobEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public SampleJobEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public HtmlDocument get_samplejob_controller()
        {
            _document.Title = "Sample Job Controller";

            _document.Add("h1").Text("Sample Job Controller");

            _document.Add("p").Text("Has executed {0} times".ToFormat(SampleJob.ExecutionCount));

            _document.Add("p")
                .Text(SampleJob.WillSucceed ? "The job will succeed on execution" : "The job fails on execution");
            _document.Push("form").Attr("action", _document.Urls.UrlFor<ToggleSampleJob>()).Attr("method", "POST");
            _document.Add("input").Attr("type", "submit").Text("Toggle the Sample Job Success State");
            _document.Pop();

            _document.Add("hr");

            _document.Push("form").Attr("action", _document.Urls.UrlFor<SetSampleJobTime>()).Attr("method", "POST");
            _document.Add("input").Attr("type", "text").Value(SampleJob.Delay.ToString()).Name("Seconds");
            _document.Add("input").Attr("type", "submit").Text("Set the execution time in seconds");


            return _document;
        }

        public FubuContinuation post_samplejob_time(SetSampleJobTime time)
        {
            SampleJob.Delay = time.Seconds;

            return FubuContinuation.RedirectTo<SampleJobEndpoint>(x => x.get_samplejob_controller());
        }

        public FubuContinuation post_samplejob_toggle(ToggleSampleJob toggle)
        {
            SampleJob.WillSucceed = !SampleJob.WillSucceed;

            return FubuContinuation.RedirectTo<SampleJobEndpoint>(x => x.get_samplejob_controller());
        }
    }

    public class ToggleSampleJob { }

    public class SetSampleJobTime
    {
        public int Seconds { get; set; }
    }
}