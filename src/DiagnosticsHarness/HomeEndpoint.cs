using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.View;
using FubuTransportation;
using HtmlTags;

namespace DiagnosticsHarness
{
    public class HomeEndpoint
    {
        private readonly IServiceBus _serviceBus;
        private readonly INumberCache _cache;
        private readonly FubuHtmlDocument _document;

        public HomeEndpoint(IServiceBus serviceBus, INumberCache cache, FubuHtmlDocument document)
        {
            _serviceBus = serviceBus;
            _cache = cache;
            _document = document;
        }

        public FubuContinuation post_numbers(NumberPost input)
        {
            var numbers =
                input.Numbers.ToDelimitedArray().Select(x => { return new NumberMessage {Value = int.Parse(x)}; });

            numbers.Each(x => _serviceBus.Send<NumberMessage>(x));

            throw new NotImplementedException("Took instrementation out");

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
            _document.Title = "FubuTransportation Diagnostics Harness";

            _document.Add("a").Attr("href", "/_fubu").Text("Diagnostics");


            return _document;
        }
    }
}