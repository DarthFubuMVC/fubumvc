using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using System.Collections.Generic;

namespace FubuMVC.Core.Http.Headers
{
    public class WriteHeadersBehavior : BasicBehavior
    {
        private readonly IHttpWriter _writer;
        private readonly IFubuRequest _request;

        public WriteHeadersBehavior(IHttpWriter writer, IFubuRequest request)
            : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            _request.Find<IHaveHeaders>()
                .SelectMany(x => x.Headers)
                .Each(x => _writer.AppendHeader(x.Name, x.Value));

            return DoNext.Continue;
        }
    }
}