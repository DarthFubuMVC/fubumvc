using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Continuations
{
    public class ContinuationHandler : BasicBehavior, IContinuationDirector
    {
        private readonly IPartialFactory _factory;
        private readonly IUrlRegistry _registry;
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public ContinuationHandler(
            IUrlRegistry registry,
            IOutputWriter writer,
            IFubuRequest request,
            IPartialFactory factory)
            : base(PartialBehavior.Ignored)
        {
            _registry = registry;
            _writer = writer;
            _request = request;
            _factory = factory;
        }


        public void InvokeNextBehavior()
        {
            if (InsideBehavior != null) InsideBehavior.Invoke();
        }

        public void RedirectTo(object input)
        {
            string url = input as string ?? _registry.UrlFor(input);
            _writer.RedirectToUrl(url);
        }

        public void RedirectToCall(ActionCall call)
        {
            string url = _registry.UrlFor(call.HandlerType, call.Method);
            _writer.RedirectToUrl(url);
        }

        public void TransferTo(object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            _request.SetObject(input);

            IActionBehavior partial = _factory.BuildPartial(input.GetType());
            partial.InvokePartial();
        }

        public void TransferToCall(ActionCall call)
        {
            IActionBehavior partial = _factory.BuildPartial(call);
            partial.InvokePartial();
        }

        protected override DoNext performInvoke()
        {
            _request.Get<FubuContinuation>().Process(this);
            return DoNext.Stop;
        }
    }
}