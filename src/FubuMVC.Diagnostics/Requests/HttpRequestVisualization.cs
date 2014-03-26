using System;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Chains;
using FubuMVC.Diagnostics.Runtime;
using HtmlTags;

namespace FubuMVC.Diagnostics.Requests
{
    public class HttpRequestVisualization : IRedirectable
    {
        private readonly BehaviorChain _chain;
        private readonly IUrlRegistry _urls;
        private readonly RequestLog _log;


        public HttpRequestVisualization(RequestLog log, BehaviorChain chain, IUrlRegistry urls)
        {
            _log = log;
            _chain = chain;
            _urls = urls;
        }

        public Guid Id { get; set; }

        public BehaviorChain Chain
        {
            get { return _chain; }
        }

        public string ChainUrl
        {
            get
            {
                return _urls.UrlFor(new ChainRequest {Id = Log.ChainId});
            }
        }

        public RequestLog Log
        {
            get { return _log; }
        }


        public HtmlTag LinksTag
        {
            get { return new RequestLinksTag(_log, _urls); }
        }

        public HtmlTag BehaviorSummary
        {
            get { return new BehaviorChainTraceTag(_chain, _log); }
        }

        public HtmlTag ResponseHeaderTag
        {
            get { return new ResponseHeadersTag(_log); }
        }

        public HtmlTag RequestLogDataOutlineTag
        {
            get { return new RequestLogDataOutlineTag(_log, _chain); }
        }

        public HtmlTag TracingOutlineTag
        {
            get { return new TracingOutlineTag(_log); }
        }

        public FubuContinuation RedirectTo { get; set; }

        public bool Equals(HttpRequestVisualization other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (HttpRequestVisualization)) return false;
            return Equals((HttpRequestVisualization) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}