using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.HelloWorld.Controllers.Conditional
{
    public class ConditionalPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.FirstCall().OutputType().CanBeCastTo<ConditionalModel>())
                .Each(chain =>
                          {
                              chain.Outputs.First().ConditionallyRunIf<CheckForQueryString>();
                              chain.FirstCall().AddAfter(new Wrapper(typeof(MiddleWare))
                                  .ConditionallyRunIf<CurrentRequest>(x => x.RawUrl.Contains("render=false")));
                          });
        }
    }
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