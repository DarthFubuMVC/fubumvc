using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class RenderJsonBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IJsonWriter _writer;

        public RenderJsonBehavior(IJsonWriter writer, IFubuRequest request)
            : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var output = _request.Get<T>();
            _writer.Write(output); 
            return DoNext.Continue;
        }
    }
}