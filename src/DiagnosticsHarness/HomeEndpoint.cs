using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.UI;
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

            _document.Add("p")
                .Text("Type in a list of comma delimited integers.  Any number over 100 will cause an exception in the message handling");

            var formTag = _document.FormFor<NumberPost>();
            _document.Push(formTag);

            _document.Add("textarea").Name("Numbers");
            _document.Add("br");
            _document.Add("input").Attr("type", "submit").Attr("value", "Submit");

            _document.Pop();
            _document.Add(new LiteralTag("</form>")); // ugh.


            return _document;
        }
    }
}