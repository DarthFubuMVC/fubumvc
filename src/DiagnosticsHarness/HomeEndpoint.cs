using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.View;
using HtmlTags;

namespace DiagnosticsHarness
{
    public class HomeEndpoint
    {
        private readonly IServiceBus _serviceBus;
        private readonly INumberCache _cache;
        private readonly FubuHtmlDocument _document;
        private readonly FubuRuntime _runtime;

        public HomeEndpoint(IServiceBus serviceBus, INumberCache cache, FubuHtmlDocument document, FubuRuntime runtime)
        {
            _serviceBus = serviceBus;
            _cache = cache;
            _document = document;
            _runtime = runtime;
        }

        public FubuContinuation post_numbers(NumberPost input)
        {
            var numbers =
                input.Numbers.ToDelimitedArray().Select(x => { return new NumberMessage {Value = int.Parse(x)}; });

            numbers.Each(x => _serviceBus.Send<NumberMessage>(x));

            return FubuContinuation.RedirectTo("_fubu");

//            return
//                FubuContinuation.RedirectTo<FubuMVC.Instrumentation.Features.Instrumentation.InstrumentationFubuDiagnostics>(
//                    x => x.get_instrumentation(null));
        }

        public string get_received()
        {
            return _cache.Captured.Select(x => x.ToString()).Join("\n");
        }

        public HtmlDocument Index()
        {
            _document.Title = "Diagnostics Harness";

            _document.Add("a").Attr("href", "/_fubu").Text("Diagnostics");
            _document.Add("br");

            _document.Add("a").Attr("href", "requests").Text("Run some fake Http Requests");

            return _document;
        }

        public FubuContinuation get_requests()
        {
            
            _serviceBus.Send(new NumberMessage{Value = 1});
            _serviceBus.Send(new NumberMessage{Value = 2});
            _serviceBus.Send(new NumberMessage{Value = 3});
            _serviceBus.Send(new NumberMessage{Value = 4});
            _serviceBus.Send(new NumberMessage{Value = 5});
            _serviceBus.Send(new NumberMessage{Value = 6});
            _serviceBus.Send(new NumberMessage{Value = 7});
            _serviceBus.Send(new NumberMessage{Value = 8});
            _serviceBus.Send(new NumberMessage{Value = 9});

            runRequests();
            runRequests();
            runRequests();
            runRequests();
            runRequests();
            runRequests();


            return FubuContinuation.RedirectTo("_fubu");
        }

        private void runRequests()
        {
            _runtime.Scenario(_ => _.Get.Url("hello"));
            _runtime.Scenario(_ =>
            {
                _.Get.Url("failure");
                _.IgnoreStatusCode();
            });
            _runtime.Scenario(_ => _.Get.Url("team/favorite"));
            _runtime.Scenario(_ =>
            {
                _.Get.Url("redirects");
                _.StatusCodeShouldBe(302);
            });


            _runtime.Scenario(_ => { _.Post.Json(new Team {City = "Denver", Mascot = "Broncos"}).Accepts("*/*"); });


            _runtime.Scenario(_ => { _.Delete.Json(new Team {City = "Denver", Mascot = "Broncos"}).Accepts("*/*"); });

            _runtime.Scenario(_ => { _.Put.Json(new Team {City = "Denver", Mascot = "Broncos"}).Accepts("*/*"); });
        }
    }
}