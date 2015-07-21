using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuTransportation;
using FubuTransportation.Diagnostics.Visualization;
using FubuTransportation.RavenDb;
using FubuTransportation.ScheduledJobs.Persistence;
using FubuTransportation.Subscriptions;
using HtmlTags;
using Process = System.Diagnostics.Process;

namespace ScheduledJobHarness
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var group = new MonitoredNodeGroup();
            group.Add("Node1", "memory://1".ToUri());
            group.Add("Node2", "memory://2".ToUri());
            group.Add("Node3", "memory://3".ToUri());
            group.Add("Node4", "memory://4".ToUri());

            group.Startup();

            var url = "http://localhost:" + group.Port;
            Process.Start(url);

            Console.WriteLine("Running the harness...");

            group.WaitForShutdown();

            Console.WriteLine("Shutting down the harness");

            group.SafeDispose();
        }
    }

    public class HomeEndpoint
    {
        private readonly FubuHtmlDocument _document;
        private readonly MonitoredNodeGroup _group;
        private readonly IUrlRegistry _urls;

        public HomeEndpoint(FubuHtmlDocument document, MonitoredNodeGroup group, IUrlRegistry urls)
        {
            _document = document;
            _group = @group;
            _urls = urls;
        }

        public HtmlDocument Index()
        {
            _document.Title = "Scheduled Job Testing Harness";

            _document.Add("p").Text("The running nodes are:");
            _document.Push("ul");

            _group.Nodes().Each(node => {
                var url = _urls.UrlFor(new ShutdownRequest {Node = node.Id});
                _document.Add("li/a").Text("Shutdown " + node.Id).Attr("href", url);
            });


            _document.Add("p/a").Text("Scheduled Job Diagnostics")
                .Attr("href", _urls.UrlFor<ScheduledJobsFubuDiagnostics>(x => x.get_scheduled_jobs()));


            _document.Add("p/a").Text("Quit the harness").Attr("href", _urls.UrlFor<HomeEndpoint>(x => x.get_quit()));

            return _document;
        }

        public FubuContinuation get_shutdown_Node(ShutdownRequest request)
        {
            _group.ShutdownNode(request.Node);

            return FubuContinuation.RedirectTo("");
        }

        public string get_quit()
        {
            _group.Shutdown();
            return "Quitting...";
        }
    }

    public class ShutdownRequest
    {
        public string Node { get; set; }
    }
}