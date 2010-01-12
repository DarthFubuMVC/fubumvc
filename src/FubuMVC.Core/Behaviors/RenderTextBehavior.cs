using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class RenderTextBehavior<T> : BasicBehavior where T : class
    {
        private readonly MimeType _mimeType;
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public RenderTextBehavior(IOutputWriter writer, IFubuRequest request, MimeType mimeType) : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
            _mimeType = mimeType;
        }

        protected override DoNext performInvoke()
        {
            _writer.Write(_mimeType.ToString(), _request.Get<T>().ToString());
            return DoNext.Continue;
        }
    }

    public class RenderHtmlBehavior : RenderTextBehavior<string>
    {
        public RenderHtmlBehavior(IOutputWriter writer, IFubuRequest request) : base(writer, request, MimeType.Html)
        {
        }
    }

    public class RenderHtmlBehavior<T> : RenderTextBehavior<T> where T : class
    {
        public RenderHtmlBehavior(IOutputWriter writer, IFubuRequest request)
            : base(writer, request, MimeType.Html)
        {
        }
    }

    
}