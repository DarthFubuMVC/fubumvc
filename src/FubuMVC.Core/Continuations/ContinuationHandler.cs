using System;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using System.Linq;

namespace FubuMVC.Core.Continuations
{
    public class ContinuationHandler : IActionBehavior, IContinuationDirector
    {
        private readonly IPartialFactory _factory;
        private readonly IChainResolver _resolver;
        private readonly IUrlRegistry _registry;
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;
        private Action _invokeNext = () => { };

        public ContinuationHandler(IUrlRegistry registry, IOutputWriter writer, IFubuRequest request, IPartialFactory factory, IChainResolver resolver)
        {
            _registry = registry;
            _writer = writer;
            _request = request;
            _factory = factory;
            _resolver = resolver;
        }

        public IActionBehavior InsideBehavior { get; set; }

        public void InvokeNextBehavior()
        {
            _invokeNext();
        }

        public void Invoke()
        {
            if (InsideBehavior != null)
            {
                _invokeNext = () => InsideBehavior.Invoke();
            }

            var continuation = FindContinuation();
            continuation.Process(this);
        }

        public void InvokePartial()
        {
            if (InsideBehavior != null)
            {
                _invokeNext = () => InsideBehavior.InvokePartial();
            }

            var continuation = FindContinuation();
            continuation.Process(this);
        }

        public void RedirectTo(object input, string categoryOrHttpMethod = null)
        {
            string url = input as string ?? _registry.UrlFor(input, categoryOrHttpMethod ?? "GET");
            _writer.RedirectToUrl(url);
        }

        public void RedirectToCall(ActionCall call, string categoryOrHttpMethod = null)
        {
            string url = _registry.UrlFor(call.HandlerType, call.Method, categoryOrHttpMethod ?? "GET");
            _writer.RedirectToUrl(url);
        }

        public void TransferTo(object input, string categoryOrHttpMethod = null)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            _request.SetObject(input);

            var chain = _resolver.FindUnique(input, categoryOrHttpMethod);

            IActionBehavior partial = _factory.BuildBehavior(chain);
            partial.InvokePartial();
        }

        public void TransferToCall(ActionCall call, string categoryOrHttpMethod = null)
        {
            var chain = _resolver.Find(call.HandlerType, call.Method, categoryOrHttpMethod);

            IActionBehavior partial = _factory.BuildBehavior(chain);
            partial.InvokePartial();
        }

        public void EndWithStatusCode(HttpStatusCode code)
        {
            _writer.WriteResponseCode(code);
        }



        public FubuContinuation FindContinuation()
        {
            var redirectable = _request.Find<IRedirectable>().FirstOrDefault();
            if (redirectable != null)
            {
                return redirectable.RedirectTo ?? FubuContinuation.NextBehavior();
            }

            return _request.Get<FubuContinuation>();
        }
    }
}