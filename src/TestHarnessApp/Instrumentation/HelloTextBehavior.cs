using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace TestHarnessApp.Instrumentation
{
    public class HelloTextBehavior: BasicBehavior
    {
        private readonly IFubuRequest _request;

        public HelloTextBehavior(IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _request = request;
        }

        protected override Task<DoNext> performInvoke()
        {
            var model = _request.Get<OtherInputModel>();

            model.HelloText = $"You said: {model.HelloText}";

            return Task.FromResult(DoNext.Continue);
        }
    }
}