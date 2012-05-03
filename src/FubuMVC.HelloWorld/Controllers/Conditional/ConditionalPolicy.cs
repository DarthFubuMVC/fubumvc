using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.HelloWorld.Controllers.Conditional
{
    public class CheckForQueryString : LambdaConditional<IFubuRequest>
    {
        public CheckForQueryString(IFubuRequest context)
            : base(context, x => x.Get<CurrentRequest>().RawUrl.Contains("render=true"))
        {
        }
    }

    public class MiddleWare : BasicBehavior
    {
        private readonly IOutputWriter _writer;

        public MiddleWare(IOutputWriter writer) : base(PartialBehavior.Executes)
        {
            _writer = writer;
        }

        protected override DoNext performInvoke()
        {
            _writer.Write("text/html", "Hello!");
            return DoNext.Continue;
        }
    }
}