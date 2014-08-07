using System;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.UI;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace TestHarnessApp.Instrumentation
{
    public class InstrumentationEndpoint
    {
        private readonly FubuHtmlDocument _document;
        private readonly IUrlRegistry _urls;

        public InstrumentationEndpoint(FubuHtmlDocument document, IUrlRegistry urls)
        {
            _document = document;
            _urls = urls;
        }

        public HtmlDocument get_instrumentation(HomeInputModel inputModel)
        {
            _document.Title = "Instrumentation Samples";
            _document.Add("h1").Text("Hello!");
            _document.Add("a").Attr("href", "/_fubu#instrumentation").Text("Go to instrumentation");

            _document.Push("ul");

            _document.Add("li/a")
                .Text("Make a really long request")
                .Attr("href", _urls.UrlFor<InstrumentationEndpoint>(x => x.get_really_long_request()));

            _document.Add("li/a")
                .Text("Throw an exception")
                .Attr("href", _urls.UrlFor<ErrorEndpoint>(x => x.get_exception()));

            _document.Add("li/a")
                .Text("Make a request with a custom behavior")
                .Attr("href", _urls.UrlFor(new OtherInputModel{HelloText = "Nothing"}));

            _document.Add("li/a")
                .Text("Make a request that occasionally throws an error")
                .Attr("href", _urls.UrlFor(new OccasionalInput{HelloText = "Hello"}));


            return _document;

        }

        [WrapWith(typeof(HelloTextBehavior))]
        public string get_other_HelloText(OtherInputModel inputModel)
        {
            return inputModel.HelloText;
        }

        public FubuContinuation get_really_long_request()
        {
            Thread.Sleep(10000);
            return FubuContinuation.RedirectTo("/instrumentation");
        }

        [WrapWith(typeof(HelloTextBehavior))]
        public HtmlDocument get_occasional_error_HelloText(OccasionalInput inputModel)
        {
            var rand = new Random();
            if (rand.Next(0, 100) < 10)
            {
                throw new Exception("Boom");
            }

            var document = new HtmlDocument();
            document.Add("h1").Text(inputModel.HelloText);

            return document;
        }
    }

    public class OccasionalInput
    {
        public string HelloText { get; set; }
    }

    public class HomeInputModel
    {
    }

    public class OtherInputModel
    {
        public string HelloText { get; set; }
    }

    public class HomeViewModel
    {
    }
}