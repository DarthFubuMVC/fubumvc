using System.Linq;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuationWriter : BasicBehavior
    {
        private readonly IJsonWriter _writer;
        private readonly IFubuRequest _request;

        public AjaxContinuationWriter(IJsonWriter writer, IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var continuation = _request.Find<AjaxContinuation>().Single();
            _writer.Write(continuation.ToDictionary(), MimeType.Json.ToString());

            return DoNext.Continue;
        }
    }
}