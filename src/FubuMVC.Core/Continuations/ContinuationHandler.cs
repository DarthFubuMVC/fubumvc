using System;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.Continuations
{
    public class ContinuationHandler : IActionBehavior, IContinuationDirector
    {
        private readonly IPartialFactory _factory;
        private readonly IChainResolver _resolver;
        private readonly IUrlRegistry _registry;
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;

        public ContinuationHandler(IUrlRegistry registry, IOutputWriter writer, IFubuRequest request, IPartialFactory factory, IChainResolver resolver)
        {
            _registry = registry;
            _writer = writer;
            _request = request;
            _factory = factory;
            _resolver = resolver;
        }

        public IActionBehavior InsideBehavior { get; set; }

        public async Task InvokeNextBehavior()
        {
            if (InsideBehavior != null)
            {
                await InsideBehavior.Invoke().ConfigureAwait(false);
            }
        }

        public async Task Invoke()
        {
            var continuation = FindContinuation();
            await continuation.Process(this).ConfigureAwait(false);
        }

        public Task InvokePartial()
        {
            var continuation = FindContinuation();
            return continuation.Process(this);
        }

        public Task RedirectTo(object input, string categoryOrHttpMethod = null)
        {
            string url = input as string ?? _registry.UrlFor(input, categoryOrHttpMethod ?? "GET");
            _writer.RedirectToUrl(url);

            // TODO -- use an async write method on the HTTP?
            return Task.CompletedTask;
        }

        public Task RedirectToCall(ActionCall call, string categoryOrHttpMethod = null)
        {
            string url = _registry.UrlFor(call.HandlerType, call.Method, categoryOrHttpMethod ?? "GET");
            _writer.RedirectToUrl(url);

            // TODO -- use an async write method on the HTTP?
            return Task.CompletedTask;
        }

        public Task TransferTo(object input, string categoryOrHttpMethod = null)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            _request.SetObject(input);

            var chain = _resolver.FindUnique(input, categoryOrHttpMethod);

            var partial = _factory.BuildBehavior(chain);
            return partial.InvokePartial();
        }

        public Task TransferToCall(ActionCall call, string categoryOrHttpMethod = null)
        {
            var chain = _resolver.Find(call.HandlerType, call.Method, categoryOrHttpMethod);

            var partial = _factory.BuildBehavior(chain);
            return partial.InvokePartial();
        }

        public Task EndWithStatusCode(HttpStatusCode code)
        {
            _writer.WriteResponseCode(code);

            return Task.CompletedTask;
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